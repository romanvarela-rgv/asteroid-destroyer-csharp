# Asteroid Destroyer

[![Build](https://github.com/romanvarela-rgv/asteroid-destroyer-csharp/actions/workflows/build.yml/badge.svg)](https://github.com/romanvarela-rgv/asteroid-destroyer-csharp/actions/workflows/build.yml)
[![Descargar](https://img.shields.io/github/v/release/romanvarela-rgv/asteroid-destroyer-csharp?label=descargar&logo=windows)](https://github.com/romanvarela-rgv/asteroid-destroyer-csharp/releases/latest)

Arcade de naves y asteroides en C# con WinForms y GDI+, sobre un motor 2D propio (`EngineGDI`). Lo hice para Paradigmas de Programación en UADE, y la idea era aplicar programación orientada a objetos en un juego de verdad, no en ejercicios sueltos.

*Asteroids-style arcade game in C# (.NET Framework 4.8, WinForms/GDI+) built on a small custom 2D engine to practice OOP, design patterns and dependency injection.*

## Qué hay adentro

| Concepto | Dónde |
|---|---|
| Herencia y polimorfismo | `Character` → `Player`, `Asteroid`, `Bullet`, `Explosion` |
| Interfaces | `IScene`, `IButton`, `ISaveStore` |
| Inyección de dependencias | Cada escena recibe sus acciones (jugar, reintentar, volver) por constructor y no toca `Program` directo; `GameManager` recibe un `ISaveStore` sin saber que es un archivo |
| Singleton | `GameManager` |
| Factory | `AsteroidFactory` (asteroides nuevos y los fragmentos al partirse) |
| Object Pool genérico | `PoolObject<T>` para las balas |
| Eventos y delegates | `Asteroid.OnDestroyed` (explosión y fragmentos), `ImageButton.OnPressed`, `Func<bool>` en `OptionsScene` |
| Enums | `GameState`, `AsteroidSize` |
| Máquina de estados | `GameState`: menú, opciones, jugando, pausa, victoria, game over |
| Archivos y excepciones | `FileSaveStore`: guarda récord, sonido y progreso en `%AppData%\AsteroidDestroyer\save.txt`, con `try/catch` para que un archivo roto no cierre el juego |

## Cómo se juega

- **3 oleadas**, cada una con más asteroides y más rápidos. Ganás al limpiar la tercera.
- Los asteroides grandes se parten en dos medianos y los medianos en dos chicos.
- Tenés **3 vidas**. Al perder una, la nave reaparece en el centro y es invulnerable unos segundos (parpadea).
- **Load Game** retoma desde el inicio de la última oleada que empezaste.
- En **Options** se prende o apaga el sonido.

## Controles

| Tecla | Acción |
|---|---|
| W A S D / flechas | Mover la nave |
| Espacio | Disparar |
| P / Esc | Pausa (Resume, Restart, Menu) |
| Enter | Elegir en los menús |
| F1 | Mostrar info de debug |

## Puntaje

Los asteroides chicos valen 100 puntos, los medianos 50 y los grandes 20. El récord queda guardado entre sesiones.

## Cómo jugarlo

1. Bajá `AsteroidDestroyer-win.zip` de la [última release](https://github.com/romanvarela-rgv/asteroid-destroyer-csharp/releases/latest).
2. Descomprimilo y abrí `EngineGDI.exe`. Necesita Windows con .NET Framework 4.8, que ya viene instalado en Windows 10 y 11.

Windows puede mostrar un aviso de SmartScreen porque el `.exe` no está firmado: tocá "Más información" y después "Ejecutar de todas formas".

## Cómo compilarlo

1. Abrí `EngineGDI.sln` con Visual Studio (Windows, .NET Framework 4.8).
2. Compilá y ejecutá con F5.

Cada push a `main` se compila solo con GitHub Actions (`.github/workflows/build.yml`). Para publicar una versión nueva, subí un tag: `git tag v1.1.0 && git push --tags`. El workflow arma el zip y crea la release.

## Assets de UI

Los botones de pausa y de opciones, los títulos ("PAUSED", "OPTIONS", "WAVE 1-3", "NEW RECORD!"), la flecha de selección y el fondo de la partida se generan con un script que copia el estilo de los botones originales (misma grilla y paleta):

```powershell
powershell -ExecutionPolicy Bypass -File tools\generate-ui-assets.ps1
```

Para sumar un botón, agregalo a `$buttonGroups` en el script y volvé a correrlo.

Todo el texto del juego (botones, títulos, HUD y menús) usa una sola fuente pixel definida en `src/PixelFont.cs`: el generador la usa para los PNG y `PixelText` para dibujar el texto en pantalla, así todo tiene las mismas letras.

---

Roman Gael Varela (RGV) · [romangaelvarela.online](https://romangaelvarela.online)
