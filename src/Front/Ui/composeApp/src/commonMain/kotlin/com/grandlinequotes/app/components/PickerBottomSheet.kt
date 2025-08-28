package com.grandlinequotes.app.components

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.outlined.Check
import androidx.compose.material.icons.outlined.ClearAll
import androidx.compose.material3.DividerDefaults
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.Icon
import androidx.compose.material3.ListItem
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.ModalBottomSheet
import androidx.compose.material3.SheetState
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.stringResource

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PickerBottomSheet(
    title: String,
    options: List<Pair<Int?, String>>,
    selectedId: Int?,
    onSelect: (Int?) -> Unit,
    onClear: () -> Unit,
    onDismiss: () -> Unit,
    state: SheetState
) {
    ModalBottomSheet(onDismissRequest = onDismiss, sheetState = state) {
        Column(Modifier.fillMaxWidth().padding(16.dp)) {
            Text(title, style = MaterialTheme.typography.titleMedium)
            Spacer(Modifier.height(8.dp))

            // Opción "Todos"
            ListItem(
                headlineContent = { Text(stringResource(Localizer.strings.filter_all)) },
                leadingContent = { Icon(Icons.Outlined.ClearAll, null) },
                trailingContent = {
                    if (selectedId == null) Icon(Icons.Outlined.Check, null)
                },
                modifier = Modifier
                    .fillMaxWidth()
                    .clickable { onClear() }
            )

            HorizontalDivider(Modifier, DividerDefaults.Thickness, DividerDefaults.color)

            LazyColumn {
                items(options) { (id, label) ->
                    ListItem(
                        headlineContent = { Text(label) },
                        trailingContent = {
                            if (id == selectedId) Icon(Icons.Outlined.Check, null)
                        },
                        modifier = Modifier
                            .fillMaxWidth()
                            .clickable { onSelect(id) }
                    )
                }
            }
            Spacer(Modifier.height(12.dp))
        }
    }
}
