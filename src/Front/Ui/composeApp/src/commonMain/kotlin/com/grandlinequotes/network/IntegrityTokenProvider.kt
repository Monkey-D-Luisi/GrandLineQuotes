package com.grandlinequotes.network

import com.grandlinequotes.api.IntegritySession

// Una única fuente para ambos interceptores
fun interface TokenProvider {
    suspend fun token(): String?
}

// Implementación usando tu IntegritySession
object IntegrityTokenProvider : TokenProvider {
    override suspend fun token(): String? =
        IntegritySession.token ?: IntegritySession.awaitToken()
}
