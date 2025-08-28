package com.grandlinequotes.api.acceptance

import com.apollographql.apollo.ApolloClient
import com.apollographql.apollo.api.http.HttpRequest
import com.apollographql.apollo.api.http.HttpResponse
import com.apollographql.apollo.network.http.HttpInterceptor
import com.apollographql.apollo.network.http.HttpInterceptorChain
import com.grandlinequotes.Config
import com.grandlinequotes.api.IQuoteService
import com.grandlinequotes.api.QuoteService
import com.grandlinequotes.network.LanguageInterceptor
import kotlinx.coroutines.test.runTest
import org.json.JSONObject
import org.koin.core.context.startKoin
import org.koin.core.context.stopKoin
import org.koin.dsl.module
import org.koin.test.KoinTest
import org.koin.test.inject
import java.net.HttpURLConnection
import java.net.URL
import java.util.UUID
import kotlin.test.AfterTest
import kotlin.test.BeforeTest
import kotlin.test.Test
import kotlin.test.assertTrue

class QuoteServiceShould : KoinTest {

    private val quoteService: IQuoteService by inject()

    @BeforeTest
    fun setup() {
        val token = fetchSessionToken()
        startKoin {
            modules(
                module {
                    single {
                        ApolloClient.Builder()
                            .serverUrl(Config.graphqlBaseUrl)
                            .addHttpInterceptor(LanguageInterceptor())
                            .addHttpInterceptor(AuthInterceptor(token))
                            .build()
                    }
                    single { QuoteService(get()) as IQuoteService }
                }
            )
        }
    }

    @AfterTest
    fun tearDown() {
        stopKoin()
    }

    @Test
    fun list_quotes_should_return_list_from_query() = runTest {
        val authorId = 1
        val arcId = 1
        val searchTerm = "hisá"

        val result = quoteService.listQuotes(authorId, arcId, searchTerm)

        assertTrue(result.isNotEmpty(), "The quote list should not be empty")
    }

    @Test
    fun get_quote_should_return_get_from_query() = runTest {
        val quoteId = 1

        val result = quoteService.getQuote(quoteId)

        assertTrue(result?.id == quoteId, "The quote should be the one that was requested")
    }
}

private fun fetchSessionToken(): String {
    val nonce = UUID.randomUUID().toString()
    val base = Config.graphqlBaseUrl.removeSuffix("/graphql")
    val url = URL(base + "/integrity/exchange")
    val conn = (url.openConnection() as HttpURLConnection).apply {
        requestMethod = "POST"
        setRequestProperty("X-Play-Integrity-Token", nonce)
        setRequestProperty("X-Play-Integrity-Nonce", nonce)
        setRequestProperty("X-Play-Integrity-Package-Name", "com.grandlinequotes.app")
        doOutput = true
    }
    val resp = conn.inputStream.bufferedReader().use { it.readText() }
    val match = Regex("\"session\"\\s*:\\s*\"([^\"]+)\"").find(resp)
    return match?.groupValues?.get(1) ?: ""
}

private class AuthInterceptor(private val token: String) : HttpInterceptor {
    override suspend fun intercept(request: HttpRequest, chain: HttpInterceptorChain): HttpResponse {
        val newRequest = request.newBuilder()
            .addHeader("Authorization", "Bearer $token")
            .build()
        return chain.proceed(newRequest)
    }
}
