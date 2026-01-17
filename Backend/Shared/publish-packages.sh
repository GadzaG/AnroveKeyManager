#!/bin/bash

# Параметры (можно менять или передавать через аргументы)
SOURCE_NAME="github"          # имя источника NuGet (как в `dotnet nuget list source`)
API_KEY="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"                       # API‑ключ для nuget.org (если требуется)

# Функция: получить PackageId из .csproj
get_package_id() {
    local csproj="$1"
    # Ищем <PackageId> в .csproj
    local package_id=$(grep -m 1 "<PackageId>" "$csproj" | sed -E 's/.*<PackageId>(.*)<\/PackageId>.*/\1/')
    if [ -n "$package_id" ]; then
        echo "$package_id"
    else
        # Если PackageId не задан — берём имя файла без .csproj
        basename "$csproj" .csproj
    fi
}

# Переходим в директорию со скриптом (чтобы работать с относительными путями)
cd "$(dirname "$0")" || exit 1

# Находим все .csproj файлы (исключая obj/ и bin/)
find . -type f -name "*.csproj" ! -path "*/obj/*" ! -path "*/bin/*" | while read -r csproj; do
    echo "=== Обрабатываю проект: $csproj ==="

    # 1. Выполняем dotnet pack
    dotnet pack "$csproj" -c Release --no-build
    if [ $? -ne 0 ]; then
        echo "❌ Сборка пакета для $csproj не удалась. Пропускаем."
        continue
    fi

    # 2. Определяем имя пакета
    package_id=$(get_package_id "$csproj")
    echo "Пакет ID: $package_id"

    # 3. Ищем nupkg файл в папке bin/Release
    nupkg_file=$(find "$(dirname "$csproj")/bin/Release" -type f -name "${package_id}*.nupkg" | sort -r | head -1)
    if [ -z "$nupkg_file" ]; then
        echo "❌ Не найден .nupkg файл для $package_id. Пропускаем."
        continue
    fi
    echo "NUPKG: $nupkg_file"

    # 4. Выполняем nuget push
    if [ -n "$API_KEY" ]; then
        dotnet nuget push "$nupkg_file" --source "$SOURCE_NAME" --api-key "$API_KEY"
    else
        dotnet nuget push "$nupkg_file" --source "$SOURCE_NAME"
    fi

    if [ $? -eq 0 ]; then
        echo "✅ Пакет $package_id успешно загружен."
    else
        echo "❌ Ошибка при загрузке пакета $package_id."
    fi

    echo ""
done

echo "Готово."
