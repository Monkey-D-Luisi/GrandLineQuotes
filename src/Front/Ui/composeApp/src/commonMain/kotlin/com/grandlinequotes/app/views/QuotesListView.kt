package com.grandlinequotes.app.views

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.material3.SnackbarHost
import androidx.compose.material3.SnackbarHostState
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import com.grandlinequotes.app.components.QuotesFilter
import com.grandlinequotes.app.components.QuotesList
import com.grandlinequotes.app.components.SupportAction
import com.grandlinequotes.app.viewModels.QuotesListViewModel
import org.koin.compose.koinInject

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun QuotesListView(
    onQuoteClick: (quoteId: Int) -> Unit,
    onBack: () -> Unit,
    onSupport: () -> Unit
) {
    val vm: QuotesListViewModel = koinInject()

    val quotes   by vm.quotes.collectAsState()
    val loading  by vm.isLoading.collectAsState()
    val authors  by vm.authors.collectAsState()
    val arcs     by vm.arcs.collectAsState()
    val errorMessage by vm.errorMessage.collectAsState()

    val snackbarHostState = remember { SnackbarHostState() }

    LaunchedEffect(errorMessage) {
        errorMessage?.let {
            snackbarHostState.showSnackbar(it)
            vm.clearError()
        }
    }

    // Primera carga (no tenemos filtros aún): spinner a pantalla completa
    val initialLoading = loading && (authors.isEmpty() || arcs.isEmpty())

    LaunchedEffect(Unit) { vm.loadQuotes() }

    Scaffold(
        contentWindowInsets = WindowInsets(top = 0, bottom = 0),
        snackbarHost = { SnackbarHost(snackbarHostState) },
        topBar = {
            TopAppBar(
                title = { Text("") },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = { SupportAction(onSupport) }
            )
        }
    ) { innerPadding ->
        if (initialLoading) {
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(innerPadding),
                contentAlignment = Alignment.Center
            ) { CircularProgressIndicator() }
        } else {
            // A partir de aquí mantenemos lista + header SIEMPRE.
            QuotesList(
                quotes = quotes,
                onQuoteClick = onQuoteClick,
                loading = loading, // mostrará una barra fina en el header si está cargando
                filterContent = { QuotesFilter(vm) },
                modifier = Modifier
                    .fillMaxSize()
                    .padding(innerPadding)
            )
        }
    }
}
