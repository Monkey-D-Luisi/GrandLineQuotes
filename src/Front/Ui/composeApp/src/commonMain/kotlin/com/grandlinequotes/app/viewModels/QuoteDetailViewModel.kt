package com.grandlinequotes.app.viewModels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.grandlinequotes.Config
import com.grandlinequotes.api.GetQuoteQuery
import com.grandlinequotes.api.IQuoteService
import androidx.compose.ui.text.intl.Locale
import co.touchlab.kermit.Logger
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class QuoteDetailViewModel(
    private val quoteService: IQuoteService,
    private val logger: Logger
) : ViewModel() {

    private val _quote = MutableStateFlow<GetQuoteQuery.Quote?>(null)
    val quote: StateFlow<GetQuoteQuery.Quote?> = _quote

    private val _videoUrl = MutableStateFlow<String?>(null)
    val videoUrl: StateFlow<String?> = _videoUrl

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage: StateFlow<String?> = _errorMessage

    fun clearError() {
        _errorMessage.value = null
    }

    fun loadQuote(quoteId: Int) {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                val result = quoteService.getQuote(quoteId)
                _quote.value = result

                _videoUrl.value = quoteId.let { id ->
                    val language = Locale.current.language
                    val dir = if (language == "es") "es" else "base"
                    "${Config.videoBaseUrl}/$dir/$id.mp4"
                }
            } catch (e: Exception) {
                logger.e(e) { "Error loading quote" }
                _errorMessage.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }
}