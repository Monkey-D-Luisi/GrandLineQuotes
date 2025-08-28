package com.grandlinequotes.app.components

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.animateContentSize
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.outlined.Tune
import androidx.compose.material3.ElevatedCard
import androidx.compose.material3.Icon
import androidx.compose.material3.IconToggleButton
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.grandlinequotes.api.ListQuotesQuery
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.stringResource

@Composable
fun QuotesList(
    quotes: List<ListQuotesQuery.Quote>,
    onQuoteClick: (Int) -> Unit,
    loading: Boolean,
    filterContent: @Composable () -> Unit,
    modifier: Modifier = Modifier
) {
    var filtersExpanded by rememberSaveable { mutableStateOf(false) }

    LazyColumn(
        modifier = modifier.padding(horizontal = 16.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        stickyHeader(key = "filters-header") {
            Column(
                Modifier
                    .fillMaxWidth()
                    .background(MaterialTheme.colorScheme.surface)
                    .animateContentSize()
            ) {
                // fila título + botón de filtros
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(top = 8.dp, bottom = 8.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = stringResource(Localizer.strings.list_title),
                        style = MaterialTheme.typography.headlineMedium,
                        modifier = Modifier.weight(1f)
                    )

                    IconToggleButton(
                        checked = filtersExpanded,
                        onCheckedChange = { filtersExpanded = it }
                    ) {
                        // usa el icono que prefieras: Tune, FilterList, FilterAlt…
                        Icon(
                            imageVector = Icons.Outlined.Tune,
                            contentDescription = if (filtersExpanded)
                                stringResource(Localizer.strings.filter_hide)
                            else
                                stringResource(Localizer.strings.filter_show)
                        )
                    }
                }

                // progreso fino (no desmonta nada)
                if (loading) {
                    LinearProgressIndicator(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(bottom = 8.dp)
                    )
                }

                // filtros plegables bajo el título
                AnimatedVisibility(visible = filtersExpanded) {
                    Column {
                        filterContent()
                        Spacer(Modifier.height(8.dp))
                    }
                }
            }
        }

        items(items = quotes, key = { it.id ?: it.hashCode() }) { quote ->
            ElevatedCard(
                modifier = Modifier
                    .fillMaxWidth()
                    .clickable { onQuoteClick(quote.id ?: 0) }
            ) {
                Column(Modifier.padding(16.dp)) {
                    Text(
                        text = quote.text ?: stringResource(Localizer.strings.common_noquote),
                        style = MaterialTheme.typography.bodyLarge
                    )
                    Spacer(Modifier.height(6.dp))
                    Text(
                        text = quote.translation ?: stringResource(Localizer.strings.common_notranslation),
                        style = MaterialTheme.typography.bodyMedium,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                    Spacer(Modifier.height(6.dp))
                    Text(
                        text = buildString {
                            append(quote.author?.name ?: stringResource(Localizer.strings.common_unknown))
                            quote.episode?.arc?.title?.let { append(" - $it") }
                        },
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.primary
                    )
                }
            }
        }

        item { Spacer(Modifier.height(12.dp)) }
    }
}


