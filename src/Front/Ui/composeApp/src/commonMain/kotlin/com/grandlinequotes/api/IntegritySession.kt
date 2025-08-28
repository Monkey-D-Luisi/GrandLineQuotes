package com.grandlinequotes.api

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.filterNotNull
import kotlinx.coroutines.flow.first

object IntegritySession {
    private val tokenFlow = MutableStateFlow<String?>(null)

    var token: String?
        get() = tokenFlow.value
        set(value) { tokenFlow.value = value }

    suspend fun awaitToken(): String = tokenFlow.filterNotNull().first()
}
