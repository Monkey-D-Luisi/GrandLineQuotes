package com.grandlinequotes.app

import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.grandlinequotes.app.components.rememberSupportActions
import com.grandlinequotes.app.views.HomeScreen
import com.grandlinequotes.app.views.QuoteDetailView
import com.grandlinequotes.app.views.QuotesListView
import com.grandlinequotes.app.views.SupportView

@Composable
fun App() {
    var currentScreen by remember { mutableStateOf<Screen>(Screen.Home) }

    MaterialTheme {
        when (val screen = currentScreen) {
            is Screen.Home -> HomeScreen(
                onQuoteClick = { quoteId -> currentScreen = Screen.QuoteDetail(quoteId) },
                onShowQuotes = { currentScreen = Screen.QuotesList },
                onSupport = { currentScreen = Screen.Support(screen) }
            )
            is Screen.QuotesList -> QuotesListView(
                onQuoteClick = { quoteId -> currentScreen = Screen.QuoteDetail(quoteId) },
                onBack = { currentScreen = Screen.Home },
                onSupport = { currentScreen = Screen.Support(screen) }
            )
            is Screen.QuoteDetail -> QuoteDetailView(
                quoteId = screen.quoteId,
                onBack = { currentScreen = Screen.QuotesList },
                onSupport = { currentScreen = Screen.Support(screen) }
            )
            is Screen.Support -> {
                val actions = rememberSupportActions()
                SupportView(
                    onBack = { currentScreen = screen.returnTo },
                    onDonate = actions.donate,
                    onRate = actions.rate,
                    onShare = actions.share
                )
            }
        }
    }
}