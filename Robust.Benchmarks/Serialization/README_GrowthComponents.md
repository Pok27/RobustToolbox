# Обновление системы роста растений в тестах RobustToolbox

## Проблема

Тесты в RobustToolbox были написаны под старый формат YAML для семян Space Station 14, где все параметры роста (nutrientConsumption, waterConsumption, idealHeat и т.д.) были отдельными полями в `SeedDataDefinition`.

В Space Station 14 была внедрена новая компонентная система роста с `growthComponents`, что привело к ошибкам десериализации в тестах.

## Решение

### 1. Обновлен SeedDataDefinition

**Старый формат:**
```yaml
- type: seed
  id: tobacco
  name: tobacco
  # ... другие поля
  nutrientConsumption: 0.25
  waterConsumption: 3.0
  idealHeat: 298
  heatTolerance: 20
  # ... другие параметры роста
```

**Новый формат:**
```yaml
- type: seed
  id: tobacco
  name: tobacco
  # ... другие поля
  growthComponents:
    - type: BasicGrowthComponent
      WaterConsumption: 3.0
      NutrientConsumption: 0.25
    - type: AtmosphericGrowthComponent
      IdealHeat: 298
      HeatTolerance: 20
      LowPressureTolerance: 25
      HighPressureTolerance: 200
    - type: WeedPestGrowthComponent
      WeedTolerance: 5
      PestTolerance: 5
```

### 2. Добавлены новые компоненты роста

Создана иерархия компонентов роста:

- `PlantGrowthComponent` (абстрактный базовый класс)
  - `BasicGrowthComponent` - базовые параметры (вода, питательные вещества)
  - `AtmosphericGrowthComponent` - атмосферные параметры (температура, давление)
  - `WeedPestGrowthComponent` - параметры сорняков и вредителей
  - `ConsumeExudeGasGrowthComponent` - газы
  - `AutoHarvestGrowthComponent` - автоматический сбор
  - `UnviableGrowthComponent` - нежизнеспособность

### 3. Обновлены тесты

Обновлены следующие тестовые файлы:
- `SerializationCopyBenchmark.cs` - убраны ссылки на старые поля, добавлено копирование компонентов
- `SerializationWriteBenchmark.cs` - обновлен baseline тест для записи компонентов
- `SerializationReadBenchmark.cs` - остается без изменений (использует обновленный прототип)

### 4. Ключевые изменения в коде

#### SeedDataDefinition.cs
```csharp
// Удалены старые поля
// [DataField("nutrientConsumption")] public float NutrientConsumption { get; set; } = 0.25f;
// [DataField("waterConsumption")] public float WaterConsumption { get; set; } = 3f;
// ... и другие

// Добавлено новое поле
[DataField("growthComponents")]
public List<PlantGrowthComponent> GrowthComponents { get; set; } = new();
```

#### PlantGrowthComponent.cs
```csharp
[RegisterComponent]
[ImplicitDataDefinitionForInheritors] // Ключевой атрибут для полиморфной десериализации
public abstract partial class PlantGrowthComponent : Component
{
    public PlantGrowthComponent DupeComponent()
    {
        return (PlantGrowthComponent)this.MemberwiseClone();
    }
}
```

## Результат

Теперь тесты RobustToolbox совместимы с новой компонентной системой роста Space Station 14 и не будут вызывать ошибки десериализации при загрузке прототипов семян.

## Проверка

Создан тестовый файл `TestGrowthComponents.cs` для проверки корректности работы новой системы.