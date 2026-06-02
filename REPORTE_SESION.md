# Reporte de Sesión — Paradigmas de Programación
**Materia:** Paradigmas de Programación — UADE  
**Proyecto:** Asteroids Destroyers (C# / GDI+)  
**Tema de la sesión:** Práctica 9 — Interfaces e Inyección de Dependencias

---

## 1. Contexto del Proyecto

El juego es un clon del clásico Asteroids desarrollado en **C# con WinForms y GDI+**, usando un motor 2D propio llamado `EngineGDI`. Ya estaban implementados los siguientes patrones de diseño:

| Patrón | Clase |
|---|---|
| Singleton | `GameManager` |
| Factory | `AsteroidFactory` |
| Object Pool genérico | `PoolObject<T>` |
| Eventos / Delegates | `Asteroid.OnDestroyed`, `Bullet.OnDeactivate`, `Menu.OnOptionSelected` |

La máquina de estados (`GameState`) manejaba los estados: `Menu`, `Playing`, `Paused`, `Victory`, `GameOver`.

---

## 2. Objetivo de la Sesión

Aplicar **Interfaces** e **Inyección de Dependencias** tal como lo indica la Práctica 9, implementándolas en el sistema de menús del juego con botones basados en imágenes PNG.

---

## 3. Concepto: ¿Qué es una Interfaz? (según la PP9)

Una interfaz define un **contrato** de comportamiento público que una clase debe cumplir. Es similar a una clase abstracta, pero:

- Una clase puede implementar **muchas interfaces** (no tiene límite como la herencia)
- Solo define la **firma** de métodos y propiedades, sin implementarlos
- Se nombran con una **"I" mayúscula** al frente (`IScene`, `IButton`)

**Ejemplo del PDF aplicado al proyecto:**  
En lugar de que `Program.cs` conozca el tipo concreto `MainMenuScene`, solo conoce `IScene`. No le importa cómo está implementada cada pantalla, solo que puede llamar `.Update()` y `.Draw()` sobre ella.

---

## 4. Concepto: Inyección de Dependencias

Es un patrón donde las dependencias (objetos de los que depende una clase) se **crean fuera** de la clase y se le **pasan como parámetro**.

**Sin inyección de dependencias** (acoplado):
```csharp
public class MainMenuScene
{
    public void Update()
    {
        // La escena sabe de Program y cambia el estado directamente — MAL
        Program.currentState = GameState.PlayMenu;
    }
}
```

**Con inyección de dependencias** (desacoplado):
```csharp
public class MainMenuScene : IScene
{
    private Action onPlay; // No sabe qué hace, solo la llama

    public MainMenuScene(Action onPlay, Action onOptions, Action onQuit)
    {
        this.onPlay = onPlay; // Se inyecta desde afuera
    }
}

// En Program.cs — quien sí sabe cambiar el estado:
mainMenuScene = new MainMenuScene(
    onPlay: () => currentState = GameState.PlayMenu,
    onQuit: () => Application.Exit()
);
```

---

## 5. Archivos Creados

### 5.1 `IScene.cs` — Interfaz de escena
```csharp
public interface IScene
{
    void Update(float deltaTime);
    void Draw();
}
```
**Rol:** Contrato que deben cumplir todas las pantallas del juego. `Program.cs` las trata a todas igual sin importar el tipo concreto.

---

### 5.2 `IButton.cs` — Interfaz de botón
```csharp
public interface IButton
{
    Transform Transform { get; set; }
    bool IsSelected { get; set; }
    event Action OnPressed;
    void Press();
    void Draw();
}
```
**Rol:** Contrato para cualquier tipo de botón. Hoy solo existe `ImageButton`, pero en el futuro podría haber `TextButton`, `AnimatedButton`, etc., sin cambiar nada en las escenas.

---

### 5.3 `ImageButton.cs` — Implementa `IButton`
Botón basado en sprite PNG con:
- Escala automática mediante `targetWidth` (ancho deseado en pixels)
- Outline cian cuando está seleccionado
- Evento `OnPressed` que dispara la acción asignada

```csharp
public class ImageButton : IButton
{
    public ImageButton(string spritePath, Vector2f position, float targetWidth = 0f)
    // targetWidth: escala el PNG nativo al ancho deseado (ej: 130px)
}
```

---

### 5.4 Cuatro clases de escena — Implementan `IScene`

| Clase | Fondo | Botones |
|---|---|---|
| `MainMenuScene` | `MainMenu.png` | Play, Options, Quit |
| `PlayMenuScene` | `PlayMenu.png` | New Game, Load Game, Back |
| `WinScene` | `WinScene.png` | Continue, Back |
| `LoseScene` | `LoseScene.png` | Retry, Back |

Cada escena:
1. Escala el fondo para llenar la pantalla 1024×544
2. Crea sus botones con `ImageButton` en posiciones calculadas desde la imagen de referencia (1920×1080)
3. Maneja navegación con teclas ↑↓ (o W/S) + Enter/Space
4. Recibe sus callbacks por constructor (inyección de dependencias)

---

## 6. Archivos Modificados

### 6.1 `GameState.cs`
Se agregó el estado `PlayMenu` para la pantalla de selección de juego:
```csharp
public enum GameState
{
    Menu,
    PlayMenu,   // ← NUEVO
    Playing,
    Paused,
    Victory,
    GameOver
}
```

### 6.2 `Program.cs`
**Antes:**
```csharp
private static Menu mainMenu; // Clase concreta

// En InitializeMenu:
mainMenu = new Menu(new List<string>{ "INICIAR JUEGO", "SALIR" }, ...);
mainMenu.OnOptionSelected += (index) => { ... };

// En Update/Draw:
case GameState.Menu:
    mainMenu.Update(deltaTime); // Acoplado al tipo concreto Menu
```

**Después:**
```csharp
private static IScene mainMenuScene; // Interfaz — no importa el tipo concreto
private static IScene playMenuScene;
private static IScene winScene;
private static IScene loseScene;

// En InitializeMenu — Inyección de dependencias:
mainMenuScene = new MainMenuScene(
    onPlay:    () => currentState = GameState.PlayMenu,
    onOptions: () => { },
    onQuit:    () => Application.Exit()
);

// En Update/Draw:
case GameState.Menu:
    mainMenuScene.Update(deltaTime); // Program.cs no sabe qué tipo es
```

---

## 7. Diagrama de Relaciones

```
IScene (interfaz)
  ├── MainMenuScene  → contiene List<IButton>
  ├── PlayMenuScene  → contiene List<IButton>
  ├── WinScene       → contiene List<IButton>
  └── LoseScene      → contiene List<IButton>

IButton (interfaz)
  └── ImageButton    → implementa con sprite PNG + outline de selección

Program.cs
  ├── IScene mainMenuScene   ←── new MainMenuScene(onPlay, onOptions, onQuit)
  ├── IScene playMenuScene   ←── new PlayMenuScene(onNewGame, onLoadGame, onBack)
  ├── IScene winScene        ←── new WinScene(onContinue, onBack)
  └── IScene loseScene       ←── new LoseScene(onRetry, onBack)
       ↑
       Inyección de dependencias: los callbacks de transición
       vienen de Program.cs, las escenas no conocen Program
```

---

## 8. Por qué esta implementación cumple la PP9

| Requisito de la PP9 | Cómo se cumple |
|---|---|
| Usar `interface` con palabra clave correcta | `IScene`, `IButton` |
| Nombre con "I" mayúscula | `IScene`, `IButton` |
| Solo firmas, sin implementación | Ambas interfaces solo declaran métodos/propiedades |
| Una clase implementa la interfaz con `:` | `ImageButton : IButton`, `MainMenuScene : IScene` |
| Clase puede heredar Y implementar interfaz | (disponible para extender, ej: `Player : Character, IDamageable`) |
| Polimorfismo vía interfaz | `Program.cs` usa `IScene` y llama Update/Draw sin conocer el tipo concreto |
| Inyección de dependencias | Callbacks `Action` pasados por constructor a cada escena |

---

## 9. Tamaños de Assets (referencia técnica)

Las escenas son imágenes de **1920×1080** escaladas a la pantalla de juego **1024×544**:
- Factor escala X: `1024 / 1920 ≈ 0.533`
- Factor escala Y: `544 / 1080 ≈ 0.504`

| PNG del botón | Tamaño nativo | targetWidth | Resultado en pantalla |
|---|---|---|---|
| PlayButton / Options / Quit | 512×256 px | 130 px | ~130×65 px |
| NewGame / LoadGame / Continue / Retry | 315×157 px | 130 px | ~130×65 px |
| Back | 219×109 px | 90 px | ~90×45 px |

---

## 10. Archivos del `.csproj` actualizados

Se registraron en `EngineGDI.csproj`:
- 7 entradas `<Compile>` para los nuevos `.cs`
- 14 entradas `<Content>` con `<CopyToOutputDirectory>Always</CopyToOutputDirectory>` para los PNG de botones y escenas

> **Nota:** En proyectos .NET Framework (a diferencia de .NET Core/5+), los archivos **no se incluyen automáticamente** en la compilación. Deben estar explícitamente listados en el `.csproj`.
