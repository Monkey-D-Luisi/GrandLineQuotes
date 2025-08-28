package com.grandlinequotes.app.components

import androidx.compose.runtime.Composable

/**
 * Holds platform specific implementations for support actions such as
 * rating, sharing or donating to the project.
 */
data class SupportActions(
    val donate: () -> Unit,
    val rate: () -> Unit,
    val share: () -> Unit,
)

@Composable
expect fun rememberSupportActions(): SupportActions
