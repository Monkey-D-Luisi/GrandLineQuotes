package com.grandlinequotes.app.components

import androidx.compose.foundation.layout.Box
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.stringResource
import org.jetbrains.compose.resources.stringResource

@Composable
fun <T> FilterDropdown(
    options: List<FilterDropdownOption<T>>,
    label: String,
    selectedId: T?,
    onSelected: (T?) -> Unit)
{
    var expanded by remember { mutableStateOf(false) }

    Box {
        TextButton(onClick = { expanded = true }) {
            Text(options.firstOrNull { it.id == selectedId }?.label ?: "${stringResource(Localizer.strings.filter_all)} $label")
        }
        DropdownMenu(expanded = expanded, onDismissRequest = { expanded = false }) {
            DropdownMenuItem(
                text = { Text(stringResource(Localizer.strings.filter_all_option)) },
                onClick = {
                    onSelected(null)
                    expanded = false
                }
            )
            options.forEach { option ->
                DropdownMenuItem(
                    text = { Text(option.label) },
                    onClick = {
                        onSelected(option.id)
                        expanded = false
                    }
                )
            }
        }
    }
}

data class FilterDropdownOption<T>(
    val id: T,
    val label: String
)


