package com.grandlinequotes.app

import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import com.grandlinequotes.app.components.SupportActions

@Composable
actual fun rememberSupportActions(): SupportActions = remember {
    SupportActions(donate = {}, rate = {}, share = {})
}
