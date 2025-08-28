package com.grandlinequotes.app.components

import androidx.compose.animation.animateContentSize
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.outlined.Book
import androidx.compose.material.icons.outlined.BookOnline
import androidx.compose.material.icons.outlined.Category
import androidx.compose.material.icons.outlined.FilterAltOff
import androidx.compose.material.icons.outlined.Person
import androidx.compose.material3.AssistChip
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Text
import androidx.compose.material3.rememberModalBottomSheetState
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.grandlinequotes.app.viewModels.QuotesListViewModel
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.pluralStringResource
import dev.icerock.moko.resources.compose.stringResource

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun QuotesFilter(viewModel: QuotesListViewModel) {
    val authors by viewModel.authors.collectAsState()
    val arcs by viewModel.arcs.collectAsState()

    var sheet by remember { mutableStateOf<Sheet?>(null) }
    val sheetState = rememberModalBottomSheetState(skipPartiallyExpanded = true)

    val clearAll = remember {
        {
            viewModel.selectedAuthorId = null
            viewModel.selectedArcId = null
            viewModel.searchTerm = ""            // deja vacío el input
            viewModel.loadQuotes()               // recarga con todo limpio
        }
    }
    val canClear = (viewModel.selectedAuthorId != null) ||
            (viewModel.selectedArcId != null) ||
            (!viewModel.searchTerm.isNullOrBlank())

    Column(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp)
            .animateContentSize()
    ) {
        // Search bar mini
        SearchField(
            value = viewModel.searchTerm.orEmpty(),
            onValueChange = {
                viewModel.searchTerm = it
                viewModel.loadQuotes()
            },
            placeholder = stringResource(Localizer.strings.filter_search_placeholder)
        )

        Spacer(Modifier.height(10.dp))

        // Chips: autor / arco
        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
            val authorLabel = viewModel.selectedAuthorId?.let { id ->
                authors.firstOrNull { it.id == id }?.name
            } ?: pluralStringResource(Localizer.plurals.common_author, 2)

            AssistChip(
                onClick = { sheet = Sheet.Authors },
                label = { Text(authorLabel) },
                leadingIcon = { Icon(Icons.Outlined.Person, null) }
            )

            val arcLabel = viewModel.selectedArcId?.let { id ->
                arcs.firstOrNull { it.id == id }?.title
            } ?: pluralStringResource(Localizer.plurals.common_arc, 2)

            AssistChip(
                onClick = { sheet = Sheet.Arcs },
                label = { Text(arcLabel) },
                leadingIcon = { Icon(Icons.Outlined.Book, null) }
            )

            Spacer(Modifier.weight(1f))

            // Chip LIMPIAR (solo si hay algo aplicado)
            if (canClear) {
                IconButton(onClick = clearAll) {
                    Icon(Icons.Outlined.FilterAltOff, contentDescription = stringResource(Localizer.strings.filter_clear))
                }
            }

        }
    }

    // Bottom sheets
    when (sheet) {
        Sheet.Authors -> PickerBottomSheet(
            title = pluralStringResource(Localizer.plurals.common_author, 2),
            options = authors.map { it.id to (it.name ?: "") },
            selectedId = viewModel.selectedAuthorId,
            onSelect = {
                viewModel.selectedAuthorId = it
                viewModel.loadQuotes()
                sheet = null
            },
            onClear = {
                viewModel.selectedAuthorId = null
                viewModel.loadQuotes()
                sheet = null
            },
            onDismiss = { sheet = null },
            state = sheetState
        )
        Sheet.Arcs -> PickerBottomSheet(
            title = pluralStringResource(Localizer.plurals.common_arc, 2),
            options = arcs.map { it.id to (it.title ?: "") },
            selectedId = viewModel.selectedArcId,
            onSelect = {
                viewModel.selectedArcId = it
                viewModel.loadQuotes()
                sheet = null
            },
            onClear = {
                viewModel.selectedArcId = null
                viewModel.loadQuotes()
                sheet = null
            },
            onDismiss = { sheet = null },
            state = sheetState
        )
        null -> Unit
    }
}

private enum class Sheet { Authors, Arcs }

