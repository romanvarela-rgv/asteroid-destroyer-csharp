# Asteroid Destroyer

Arcade de naves y asteroides en C# con WinForms y GDI+, sobre un motor 2D propio (`EngineGDI`). Lo hice para Paradigmas de Programación en UADE, y la idea era aplicar programación orientada a objetos en un juego de verdad, no en ejercicios sueltos.

*Asteroids-style arcade game in C# (.NET Framework 4.8, WinForms/GDI+) built on a small custom 2D engine to practice OOP, design patterns and dependency injection.*

## Qué hay adentro

| Concepto | Dónde |
|---|---|
| Herencia y polimorfismo | `Character` → `Player`, `Asteroid`, `Bullet` |
| Interfaces | `IScene`, `IButton` |
| Inyección de dependencias | Cada escena recibe sus acciones (jugar, reintentar, volver) por constructor y no toca `Program` directo |
| Singleton | `GameManager` |
| Factory | `AsteroidFactory` |
| Object Pool genérico | `ObjectPool`, `BulletPool` |
| Eventos y delegates | `Asteroid.OnDestroyed`, `Bullet.OnDeactivate`, `Menu.OnOptionSelected` |
| Máquina de estados | `GameState`: menú, jugando, pausa, victoria, game over |

En [`REPORTE_SESION.md`](REPORTE_SESION.md) explico paso a paso cómo pasé el sistema de menús a interfaces e inyección de dependencias.

## Controles

| Tecla | Acción |
|---|---|
| W A S D / flechas | Mover la nave |
| Espacio | Disparar |
| P | Pausa |
| Enter | Elegir en los menús |

## Cómo correrlo

1. Abrí `EngineGDI.sln` con Visual Studio (Windows, .NET Framework 4.8).
2. Compilá y ejecutá con F5.

---

Roman Gael Varela (RGV) · [romangaelvarela.online](https://romangaelvarela.online)
