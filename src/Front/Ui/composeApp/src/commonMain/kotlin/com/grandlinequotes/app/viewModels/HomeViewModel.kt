package com.grandlinequotes.app.viewModels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.grandlinequotes.api.IQuoteService
import com.grandlinequotes.api.ListQuotesQuery
import co.touchlab.kermit.Logger
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class HomeViewModel(
    private val quoteService: IQuoteService,
    private val logger: Logger
) : ViewModel() {

    private val _randomQuote = MutableStateFlow<ListQuotesQuery.Quote?>(null)
    val randomQuote: StateFlow<ListQuotesQuery.Quote?> = _randomQuote

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage: StateFlow<String?> = _errorMessage

    fun clearError() {
        _errorMessage.value = null
    }

    fun loadRandomQuote() {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                val result = quoteService.listQuotes(null, null, null)

                _randomQuote.value = result.random()
            } catch (e: Exception) {
                logger.e(e) { "Error loading random quote" }
                _errorMessage.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }
}