package com.grandlinequotes.network

import com.apollographql.apollo.api.http.HttpRequest
import com.apollographql.apollo.api.http.HttpResponse
import com.apollographql.apollo.network.http.HttpInterceptor
import com.apollographql.apollo.network.http.HttpInterceptorChain
import com.grandlinequotes.api.IntegritySession

class ApolloAuthInterceptor(
    private val provider: TokenProvider = IntegrityTokenProvider
) : HttpInterceptor {
    override suspend fun intercept(
        request: HttpRequest,
        chain: HttpInterceptorChain
    ): HttpResponse {
        val t = provider.token()
        val newReq = request.newBuilder().apply {
            if (!t.isNullOrEmpty()) addHeader("Authorization", "Bearer $t")
        }.build()
        return chain.proceed(newReq)
    }
}
