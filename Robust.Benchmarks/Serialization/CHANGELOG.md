# Changelog: Обновление тестов RobustToolbox под новую систему роста растений

## Изменения в RobustToolbox

### Обновленные файлы:

1. **Robust.Benchmarks/Serialization/Definitions/SeedDataDefinition.cs**
   - Обновлен YAML прототип под новый формат с `growthComponents`
   - Удалены старые поля роста (nutrientConsumption, waterConsumption, idealHeat и т.д.)
   - Добавлено поле `GrowthComponents` типа `List<PlantGrowthComponent>`
   - Добавлены все компоненты роста с правильными атрибутами

2. **Robust.Benchmarks/Serialization/Copy/SerializationCopyBenchmark.cs**
   - Обновлен метод `BaselineCreateCopySeedDataDefinition()`
   - Убраны ссылки на старые поля роста
   - Добавлено копирование компонентов роста через `DupeComponent()`

3. **Robust.Benchmarks/Serialization/Write/SerializationWriteBenchmark.cs**
   - Обновлен метод `BaselineWriteSeedDataDefinition()`
   - Добавлена запись компонентов роста в YAML формат
   - Реализована поддержка всех типов компонентов

4. **Robust.Benchmarks/Serialization/TestGrowthComponents.cs** (новый файл)
   - Тестовый файл для проверки корректности работы новой системы

5. **Robust.Benchmarks/Serialization/README_GrowthComponents.md** (новый файл)
   - Документация по изменениям

## Ключевые моменты для Space Station 14

### 1. Атрибуты для полиморфной десериализации

Убедитесь, что в Space Station 14 базовый класс `PlantGrowthComponent` имеет атрибут `[ImplicitDataDefinitionForInheritors]`:

```csharp
[RegisterComponent]
[ImplicitDataDefinitionForInheritors] // ОБЯЗАТЕЛЬНО!
public abstract partial class PlantGrowthComponent : Component
{
    // ...
}
```

### 2. Атрибуты для наследников

Все наследники должны иметь только `[RegisterComponent]` (без `[ImplicitDataDefinitionForInheritors]`):

```csharp
[RegisterComponent] // Только этот атрибут
public sealed partial class BasicGrowthComponent : PlantGrowthComponent
{
    [DataField]
    public float WaterConsumption = 0.5f;
    
    [DataField]
    public float NutrientConsumption = 0.75f;
}
```

### 3. YAML формат

В файлах `seeds.yml` используйте новый формат:

```yaml
- type: seed
  id: wheat
  name: seeds-wheat-name
  # ... другие поля
  growthComponents:
    - type: BasicGrowthComponent
      WaterConsumption: 0.5
      NutrientConsumption: 0.4
    - type: AtmosphericGrowthComponent
      IdealHeat: 298
      HeatTolerance: 20
      LowPressureTolerance: 25
      HighPressureTolerance: 200
    - type: WeedPestGrowthComponent
      WeedTolerance: 5
      PestTolerance: 5
  # ... остальные поля
```

### 4. Обновление SeedPrototype

В `SeedPrototype` замените старые поля на новое:

```csharp
// Удалить старые поля
// public float NutrientConsumption { get; set; }
// public float WaterConsumption { get; set; }
// public float IdealHeat { get; set; }
// ... и другие

// Добавить новое поле
[DataField]
public List<PlantGrowthComponent> GrowthComponents = new();
```

## Проверка изменений

1. Перезапустите сервер Space Station 14
2. Проверьте, что прототипы семян загружаются без ошибок
3. Убедитесь, что все компоненты роста работают корректно

## Обратная совместимость

Если нужно поддерживать старый формат, можно добавить миграцию или поддержку обоих форматов в `SeedPrototype`.