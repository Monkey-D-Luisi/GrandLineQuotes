package com.grandlinequotes.network

import kotlinx.coroutines.runBlocking
import okhttp3.Interceptor
import okhttp3.Response

class OkHttpAuthInterceptor(
    private val provider: TokenProvider = IntegrityTokenProvider
) : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        // OkHttp ya trabaja en background; bloquear breve está bien
        val t = runBlocking { provider.token() }
        val req = chain.request().newBuilder().apply {
            if (!t.isNullOrEmpty()) header("Authorization", "Bearer $t")
        }.build()
        return chain.proceed(req)
    }
}
