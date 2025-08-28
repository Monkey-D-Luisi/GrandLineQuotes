package com.grandlinequotes.app

sealed class Screen {
    object Home : Screen()
    object QuotesList : Screen()
    data class QuoteDetail(val quoteId: Int) : Screen()
    data class Support(val returnTo: Screen) : Screen()
}
