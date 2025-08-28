package com.grandlinequotes.app.views

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import com.grandlinequotes.app.components.QuoteDetail
import com.grandlinequotes.app.components.SupportAction
import com.grandlinequotes.app.viewModels.QuoteDetailViewModel
import org.koin.compose.koinInject

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun QuoteDetailView(
    quoteId: Int,
    onBack: () -> Unit,
    onSupport: () -> Unit
) {
    val viewModel: QuoteDetailViewModel = koinInject()
    val quote by viewModel.quote.collectAsState()
    val videoUrl by viewModel.videoUrl.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()

    LaunchedEffect(Unit) {
        viewModel.loadQuote(quoteId)
    }

    Column {
        TopAppBar(
            title = { Text("") },
            navigationIcon = {
                IconButton(onClick = onBack) {
                    Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                }
            },
            actions = { SupportAction(onSupport) }
        )

        if (isLoading) {
            Box(
                modifier = Modifier.fillMaxSize(),
                contentAlignment = Alignment.Center
            ) {
                CircularProgressIndicator()
            }
        }
        else {
            QuoteDetail(quote, videoUrl)
        }
    }
}
