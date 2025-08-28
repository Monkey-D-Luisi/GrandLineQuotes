import dev.icerock.gradle.MRVisibility
import org.jetbrains.compose.desktop.application.dsl.TargetFormat
import org.jetbrains.kotlin.gradle.ExperimentalKotlinGradlePluginApi
import org.jetbrains.kotlin.gradle.ExperimentalWasmDsl
import org.jetbrains.kotlin.gradle.dsl.JvmTarget
import org.jetbrains.kotlin.gradle.targets.js.webpack.KotlinWebpackConfig
import com.codingfeline.buildkonfig.compiler.FieldSpec.Type.STRING

plugins {
    alias(libs.plugins.kotlinMultiplatform)
    alias(libs.plugins.androidApplication)
    alias(libs.plugins.composeMultiplatform)
    alias(libs.plugins.composeCompiler)
    alias(libs.plugins.composeHotReload)
    id("com.apollographql.apollo") version "4.3.2"
    id("dev.icerock.mobile.multiplatform-resources") version "0.25.0"
    id("com.codingfeline.buildkonfig") version "0.17.1"
}

/** ========= SEGUIMOS USANDO -Pflavor SOLO PARA Apollo/BuildKonfig (no Android flavors) ========= */
val flavor: String = project.findProperty("flavor")?.toString() ?: "Local"
data class EnvConfig(val graphqlEndpoint: String, val apiBaseUrl: String, val videoBaseUrl: String)
val envConfig = when (flavor) {
    "Local" -> EnvConfig(
        graphqlEndpoint = "https://grandlinequotes-api-public-dev.local:8443/graphql",
        apiBaseUrl = "https://grandlinequotes-api-public-dev.local:8443",
        videoBaseUrl = "https://grandlinequotes-api-public-dev.local:8443/videos"
    )
    "Sandbox" -> EnvConfig(
        graphqlEndpoint = "http://localhost:5024/graphql",
        apiBaseUrl = "http://localhost:5024",
        videoBaseUrl = "http://localhost:5024/videos"
    )
    "Test" -> EnvConfig(
        graphqlEndpoint = "https://public.95-217-6-136.sslip.io/graphql",
        apiBaseUrl = "https://public.95-217-6-136.sslip.io",
        videoBaseUrl = "https://public.95-217-6-136.sslip.io/videos"
    )
    else -> error("Unknown flavor $flavor")
}
val graphqlEndpoint = envConfig.graphqlEndpoint
val apiBaseUrl = envConfig.apiBaseUrl
val videoBaseUrl = envConfig.videoBaseUrl

apollo {
    service("api") {
        packageName.set("com.grandlinequotes.api")
        srcDir("src/commonMain/graphql")
        introspection {
            endpointUrl.set(graphqlEndpoint) // se decide con -Pflavor
            schemaFile.set(file("src/commonMain/graphql/com/grandlinequotes/api/schema.graphqls"))
        }
    }
}

/** BuildKonfig: sigue sirviendo a common/ios/desktop/wasm. Android en runtime leerá de BuildConfig de flavors. */
buildkonfig {
    packageName = "com.grandlinequotes"
    defaultConfigs {
        buildConfigField(STRING, "ENVIRONMENT", flavor)
        buildConfigField(STRING, "API_BASE_URL", apiBaseUrl)
        buildConfigField(STRING, "VIDEO_BASE_URL", videoBaseUrl)
    }
}

kotlin {
    androidTarget {
        @OptIn(ExperimentalKotlinGradlePluginApi::class)
        compilerOptions { jvmTarget.set(JvmTarget.JVM_11) }
    }

    listOf(
        iosX64(),
        iosArm64(),
        iosSimulatorArm64()
    ).forEach { iosTarget ->
        iosTarget.binaries.framework {
            baseName = "ComposeApp"
            isStatic = true
        }
    }

    jvm("desktop")

    @OptIn(ExperimentalWasmDsl::class)
    wasmJs {
        outputModuleName.set("composeApp")
        browser {
            val rootDirPath = project.rootDir.path
            val projectDirPath = project.projectDir.path
            commonWebpackConfig {
                outputFileName = "composeApp.js"
                devServer = (devServer ?: KotlinWebpackConfig.DevServer()).apply {
                    static = (static ?: mutableListOf()).apply {
                        add(rootDirPath)
                        add(projectDirPath)
                    }
                }
            }
        }
        binaries.executable()
    }

    sourceSets {
        val desktopMain by getting

        androidMain.dependencies {
            implementation(compose.preview)
            implementation(libs.androidx.activity.compose)
            implementation("io.insert-koin:koin-android:4.1.0")
            /** Billing v8 → SOLO Android */
            implementation("com.android.billingclient:billing-ktx:8.0.0")
            implementation("com.google.android.play:integrity:1.4.0")
            implementation("com.squareup.okhttp3:okhttp:5.1.0")
            implementation("com.squareup.okhttp3:okhttp-dnsoverhttps:5.1.0")
            implementation("androidx.media3:media3-exoplayer:1.8.0")
            implementation("androidx.media3:media3-ui:1.8.0")
            implementation("androidx.media3:media3-datasource-okhttp:1.8.0")
            implementation("com.squareup.okhttp3:okhttp:5.1.0")
        }
        commonMain.dependencies {
            implementation(compose.runtime)
            implementation(compose.foundation)
            implementation(compose.material3)
            implementation(compose.ui)
            implementation(compose.components.resources)
            implementation(compose.components.uiToolingPreview)
            implementation(libs.androidx.lifecycle.viewmodel)
            implementation(libs.androidx.lifecycle.runtimeCompose)
            implementation("org.jetbrains.kotlinx:kotlinx-coroutines-test:1.10.2")
            implementation("com.apollographql.apollo:apollo-runtime:4.3.2")
            implementation("io.insert-koin:koin-core:4.1.0")
            implementation("io.insert-koin:koin-compose:4.1.0")
            implementation("io.insert-koin:koin-test:4.1.0")
            implementation("org.jetbrains.compose.material:material-icons-extended:1.7.3")
            implementation("dev.icerock.moko:resources:0.25.0")
            implementation("dev.icerock.moko:resources-compose:0.25.0")
            /** OJO: Billing estaba aquí antes; se ha movido a androidMain para no romper iOS/desktop/wasm */
            implementation("co.touchlab:kermit:2.0.4")
        }
        commonTest.dependencies {
            implementation(libs.kotlin.test)
            implementation("dev.icerock.moko:resources-test:0.25.0")
        }
        desktopMain.dependencies {
            implementation(compose.desktop.currentOs)
            implementation(libs.kotlinx.coroutinesSwing)
        }
    }
}

android {
    namespace = "com.grandlinequotes.app"
    compileSdk = libs.versions.android.compileSdk.get().toInt()

    defaultConfig {
        applicationId = "com.grandlinequotes.app"
        minSdk = libs.versions.android.minSdk.get().toInt()
        targetSdk = libs.versions.android.targetSdk.get().toInt()
        versionCode = 1
        versionName = "1.0"
    }

    buildFeatures {
        buildConfig = true
    }

    /** ========= AÑADIMOS FLAVORS ANDROID (Local / Sandbox / Test) ========= */
    flavorDimensions += listOf("env")
    productFlavors {
        create("Local") {
            dimension = "env"
            applicationIdSuffix = ".local"
            versionNameSuffix = "-local"
            buildConfigField("String", "GRAPHQL_ENDPOINT", "\"https://grandlinequotes-api-public-dev.local:8443/graphql\"")
            buildConfigField("String", "API_BASE_URL",     "\"https://grandlinequotes-api-public-dev.local:8443\"")
            buildConfigField("String", "VIDEO_BASE_URL",   "\"https://grandlinequotes-api-public-dev.local:8443/videos\"")
        }
        create("Sandbox") {
            dimension = "env"
            applicationIdSuffix = ".sandbox"
            versionNameSuffix = "-sandbox"
            buildConfigField("String", "GRAPHQL_ENDPOINT", "\"http://localhost:5024/graphql\"")
            buildConfigField("String", "API_BASE_URL",     "\"http://localhost:5024\"")
            buildConfigField("String", "VIDEO_BASE_URL",   "\"http://localhost:5024/videos\"")
        }
        create("Test") {
            dimension = "env"
            versionNameSuffix = "-test"
            buildConfigField("String", "GRAPHQL_ENDPOINT", "\"https://public.95-217-6-136.sslip.io/graphql\"")
            buildConfigField("String", "API_BASE_URL",     "\"https://public.95-217-6-136.sslip.io\"")
            buildConfigField("String", "VIDEO_BASE_URL",   "\"https://public.95-217-6-136.sslip.io/videos\"")
        }
    }

    packaging {
        resources { excludes += "/META-INF/{AL2.0,LGPL2.1}" }
    }

    buildTypes {
        getByName("release") {
            ndk {
                debugSymbolLevel = "SYMBOL_TABLE"
            }

            isMinifyEnabled = true

            isShrinkResources = true

            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
        getByName("debug")   { isDebuggable = true }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_11
        targetCompatibility = JavaVersion.VERSION_11
    }
}

dependencies {
    debugImplementation(compose.uiTooling)
}

compose.desktop {
    application {
        mainClass = "com.grandlinequotes.app.MainKt"
        nativeDistributions {
            targetFormats(TargetFormat.Dmg, TargetFormat.Msi, TargetFormat.Deb)
            packageName = "com.grandlinequotes.app"
            packageVersion = "1.0.0"
        }
    }
}

multiplatformResources {
    resourcesPackage.set("com.grandlinequotes.resources")
    resourcesClassName.set("Localizer")
    iosBaseLocalizationRegion.set("en")
}
