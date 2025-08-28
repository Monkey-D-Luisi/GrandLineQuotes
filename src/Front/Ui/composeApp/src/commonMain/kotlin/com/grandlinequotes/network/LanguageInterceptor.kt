package com.grandlinequotes.network

import androidx.compose.ui.text.intl.Locale
import com.apollographql.apollo.api.http.HttpRequest
import com.apollographql.apollo.api.http.HttpResponse
import com.apollographql.apollo.network.http.HttpInterceptor
import com.apollographql.apollo.network.http.HttpInterceptorChain

class LanguageInterceptor : HttpInterceptor {
    override suspend fun intercept(request: HttpRequest, chain: HttpInterceptorChain): HttpResponse {
        val language = Locale.current.language
        val newRequest = request.newBuilder()
            .addHeader("Accept-Language", language)
            .build()
        return chain.proceed(newRequest)
    }
}