# Ui

Mobile client written in Kotlin with Compose. While the project keeps its original multiplatform structure, only the **Android** variant is currently functional.

## Flavors and configuration

API URLs and other settings are defined through product flavors in [`composeApp/build.gradle.kts`](composeApp/build.gradle.kts). Three main flavors exist:

- `Local`: targets the stack started with Docker Compose locally.
- `Sandbox`: used by the test script `src/Scripts/Sandbox/run-tests.sh`.
- `Test`: points to the remote environment consumed by the published app.

To build an APK for a specific flavor:

```bash
./gradlew :composeApp:assemble<Flavor>Debug
```

Example for the Test environment:

```bash
./gradlew :composeApp:assembleTestDebug
```
