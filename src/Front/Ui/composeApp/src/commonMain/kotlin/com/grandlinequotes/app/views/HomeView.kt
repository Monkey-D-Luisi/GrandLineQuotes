package com.grandlinequotes.app.views

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.WindowInsetsSides
import androidx.compose.foundation.layout.calculateEndPadding
import androidx.compose.foundation.layout.calculateStartPadding
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.only
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.sizeIn
import androidx.compose.foundation.layout.statusBars
import androidx.compose.foundation.layout.systemBars
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalLayoutDirection
import androidx.compose.ui.unit.dp
import com.grandlinequotes.app.components.HomeQuoteCard
import com.grandlinequotes.app.components.SupportAction
import com.grandlinequotes.app.viewModels.HomeViewModel
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.stringResource
import grandlinequotes.composeapp.generated.resources.Res
import grandlinequotes.composeapp.generated.resources.ic_app_icon
import org.jetbrains.compose.resources.painterResource
import org.koin.compose.koinInject

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun HomeScreen(
    onQuoteClick: (quoteId: Int) -> Unit,
    onShowQuotes: () -> Unit,
    onSupport: () -> Unit
) {
    val viewModel: HomeViewModel = koinInject()
    val randomQuote by viewModel.randomQuote.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()

    LaunchedEffect(Unit) { viewModel.loadRandomQuote() }

    Scaffold(
        contentWindowInsets = WindowInsets.systemBars
            .only(WindowInsetsSides.Horizontal + WindowInsetsSides.Bottom),
        topBar = {
            TopAppBar(
                title = { Text("") },
                actions = { SupportAction(onSupport) },
                windowInsets = WindowInsets.statusBars,
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = Color.Transparent,
                    scrolledContainerColor = Color.Transparent
                )
            )
        }
    ) { innerPadding ->
        val contentPadding = PaddingValues(
            start  = 16.dp,
            end    = 16.dp,
            bottom = 16.dp,
            top    = 56.dp
        )

        BoxWithConstraints(
            modifier = Modifier
                .fillMaxSize()
                .padding(contentPadding)
        ) {
            val isLandscape = maxWidth > maxHeight

            if (isLandscape) {
                val showArtwork = maxWidth > 600.dp

                Row(
                    modifier = Modifier.fillMaxSize(),
                    verticalAlignment = Alignment.Top,
                    horizontalArrangement = Arrangement.spacedBy(24.dp)
                ) {
                    // Columna izquierda (texto + quote)
                    Column(
                        modifier = Modifier
                            .weight(1f)
                            .fillMaxHeight(),
                        horizontalAlignment = Alignment.Start
                    ) {
                        // Header con CTA a la derecha
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Column(modifier = Modifier.weight(1f)) {
                                Text(
                                    text = stringResource(Localizer.strings.home_title),
                                    style = MaterialTheme.typography.headlineMedium
                                )
                                Text(
                                    text = stringResource(Localizer.strings.home_subtitle),
                                    style = MaterialTheme.typography.bodyMedium,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }
                            Button(
                                onClick = onShowQuotes,
                                shape = RoundedCornerShape(24.dp),
                                contentPadding = PaddingValues(horizontal = 18.dp, vertical = 10.dp)
                            ) {
                                Text(stringResource(Localizer.strings.home_button_show))
                            }
                        }

                        Spacer(Modifier.height(12.dp))

                        // La quote ocupa el resto del alto
                        Box(
                            modifier = Modifier
                                .weight(1f)
                                .fillMaxWidth(),
                            contentAlignment = Alignment.Center
                        ) {
                            if (isLoading) {
                                CircularProgressIndicator()
                            } else {
                                HomeQuoteCard(randomQuote, onQuoteClick)
                            }
                        }
                    }

                    // Columna derecha (imagen) — sólo si hay ancho suficiente
                    if (showArtwork) {
                        Image(
                            painter = painterResource(Res.drawable.ic_app_icon),
                            contentDescription = null,
                            modifier = Modifier
                                .padding(top = 16.dp)
                                .sizeIn(maxWidth = 128.dp, maxHeight = 128.dp)
                                .align(Alignment.CenterVertically)
                        )
                    }
                }
            } else {
                // --- PORTRAIT: similar al tuyo, con el bloque central a weight(1f) ---
                Column(
                    modifier = Modifier.fillMaxSize(),
                    horizontalAlignment = Alignment.CenterHorizontally,
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    Column(horizontalAlignment = Alignment.Start) {
                        Text(
                            text = stringResource(Localizer.strings.home_title),
                            style = MaterialTheme.typography.headlineMedium
                        )
                        Text(
                            text = stringResource(Localizer.strings.home_subtitle),
                            style = MaterialTheme.typography.bodyMedium,
                            color = Color.Gray
                        )
                    }

                    Box(
                        modifier = Modifier
                            .weight(1f)
                            .fillMaxWidth(),
                        contentAlignment = Alignment.Center
                    ) {
                        if (isLoading) {
                            CircularProgressIndicator()
                        } else {
                            HomeQuoteCard(randomQuote, onQuoteClick)
                        }
                    }

                    Button(
                        onClick = onShowQuotes,
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Text(stringResource(Localizer.strings.home_button_show))
                    }

                    Spacer(Modifier.height(24.dp))

                    Image(
                        painter = painterResource(Res.drawable.ic_app_icon),
                        contentDescription = null,
                        modifier = Modifier.size(128.dp)
                    )
                }
            }
        }
    }
}