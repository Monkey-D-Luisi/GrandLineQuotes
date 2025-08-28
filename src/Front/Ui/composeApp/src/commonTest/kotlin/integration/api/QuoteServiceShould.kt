package com.grandlinequotes.api.integration

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
import java.net.HttpURLConnection
import java.net.URL
import java.util.UUID
import kotlin.test.BeforeTest
import kotlin.test.Test
import kotlin.test.assertTrue

class QuoteServiceShould {

    private lateinit var quoteService: IQuoteService

    @BeforeTest
    fun setup() {
        val token = fetchSessionToken()
        val apolloClient = ApolloClient.Builder()
            .serverUrl(Config.graphqlBaseUrl)
            .addHttpInterceptor(LanguageInterceptor())
            .addHttpInterceptor(AuthInterceptor(token))
            .build()
        quoteService = QuoteService(apolloClient)
    }

    @Test
    fun list_quotes_returns_data() = runTest {
        val result = quoteService.listQuotes(1, 1, "hisá")
        assertTrue(result.isNotEmpty(), "The quote list should not be empty")
    }

    @Test
    fun get_quote_returns_item() = runTest {
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
