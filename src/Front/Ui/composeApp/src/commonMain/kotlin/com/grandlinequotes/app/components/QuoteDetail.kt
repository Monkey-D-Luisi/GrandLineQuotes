package com.grandlinequotes.app.components

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clipToBounds
import androidx.compose.ui.unit.dp
import com.grandlinequotes.api.GetQuoteQuery
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.stringResource

@Composable
fun QuoteDetail(
    quote: GetQuoteQuery.Quote?,
    videoUrl: String?,
) {
    BoxWithConstraints(Modifier.fillMaxSize()) {
        val isLandscape = maxWidth > maxHeight
        val isWide = maxWidth >= 600.dp   // umbral “tablet” aproximado

        when {
            isLandscape && isWide -> TwoPaneDetail(quote, videoUrl)
            isLandscape -> LandscapePhoneDetail(quote, videoUrl)
            else -> PortraitDetail(quote, videoUrl)
        }
    }
}

@Composable
private fun PortraitDetail(quote: GetQuoteQuery.Quote?, videoUrl: String?) {
    Column(
        Modifier.fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        TitleBlock(quote)
        Spacer(Modifier.height(16.dp))
        if (videoUrl != null) {
            VideoBox(
                videoUrl,
                Modifier.fillMaxWidth().aspectRatio(16f / 9f) // altura estable
            )
            Spacer(Modifier.height(16.dp))
        }
        TranslationBlock(quote)
    }
}

@Composable
private fun LandscapePhoneDetail(quote: GetQuoteQuery.Quote?, videoUrl: String?) {
    Column(
        Modifier.fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        TitleBlock(quote)
        Spacer(Modifier.height(12.dp))
        if (videoUrl != null) {
            VideoBox(
                videoUrl,
                Modifier.fillMaxWidth().aspectRatio(16f / 9f)
            )
            Spacer(Modifier.height(16.dp))
        }
        TranslationBlock(quote)
    }
}

@Composable
private fun TwoPaneDetail(quote: GetQuoteQuery.Quote?, videoUrl: String?) {
    Row(
        Modifier.fillMaxSize().padding(horizontal = 16.dp, vertical = 12.dp),
        verticalAlignment = Alignment.Top
    ) {
        // Panel izquierdo
        Column(
            Modifier.weight(1f),
            horizontalAlignment = Alignment.Start
        ) {
            if (videoUrl != null) {
                // -> ancho del panel, altura 16:9 (no se sale)
                VideoBox(
                    videoUrl = videoUrl,
                    modifier = Modifier
                        .fillMaxWidth()
                        .aspectRatio(16f / 9f)           // ¡sin matchHeightConstraintsFirst!
                )
            } else {
                Spacer(Modifier
                    .fillMaxWidth()
                    .aspectRatio(16f / 9f))
            }
        }

        Spacer(Modifier.width(24.dp))

        // Panel derecho
        Column(
            Modifier
                .weight(1f)
                .fillMaxHeight()
                .verticalScroll(rememberScrollState())
        ) {
            TitleBlock(quote)
            Spacer(Modifier.height(16.dp))
            TranslationBlock(quote)
        }
    }
}

@Composable
private fun TitleBlock(quote: GetQuoteQuery.Quote?) {
    Text(
        quote?.originalText ?: stringResource(Localizer.strings.common_nooriginal),
        style = MaterialTheme.typography.headlineMedium
    )
    Text(
        quote?.text ?: stringResource(Localizer.strings.common_noquote),
        style = MaterialTheme.typography.headlineMedium
    )
    Spacer(Modifier.height(8.dp))
    quote?.author?.name?.let { Text(it, style = MaterialTheme.typography.bodyLarge) }
    quote?.episode?.let {
        Text("${it.number} - ${it.title}", style = MaterialTheme.typography.bodyMedium)
        listOfNotNull(it.arc?.title, it.arc?.saga?.title).takeIf { l -> l.isNotEmpty() }?.let { l ->
            Text(l.joinToString(" – "), style = MaterialTheme.typography.bodySmall, color = MaterialTheme.colorScheme.primary)
        }
    }
}

@Composable
private fun TranslationBlock(quote: GetQuoteQuery.Quote?) {
    Text(
        quote?.translation ?: stringResource(Localizer.strings.common_notranslation),
        style = MaterialTheme.typography.headlineMedium
    )
}

@Composable
private fun VideoBox(videoUrl: String, modifier: Modifier) {
    Box(modifier) {
        // contenedor con clip
        Box(Modifier.fillMaxSize().clipToBounds()) {
            VideoPlayer(videoUrl = videoUrl, modifier = Modifier.fillMaxSize())
        }
    }
}

