# 🏓 TP03: Pong 2D - Físicas y Personalización

Trabajo Práctico desarrollado en **Unity** que implementa el clásico juego **Pong** en dos dimensiones, integrando simulación física (`Rigidbody2D` / `AddForce`), uso estricto de `Delta Time`, aumento de velocidad por impacto y un sistema dinámico de opciones para personalizar el juego en tiempo real.

---

## 🎮 Controles de Juego

| Jugador / Acción | Control | Descripción |
| :--- | :---: | :--- |
| **Jugador 1 (Izquierda)** | <kbd>W</kbd> / <kbd>S</kbd> | Mover la paleta hacia Arriba / Abajo |
| **Jugador 2 (Derecha)** | <kbd>▲</kbd> / <kbd>▼</kbd> (Flechas) | Mover la paleta hacia Arriba / Abajo |
| **Pausa / Menú** | <kbd>Escape</kbd> | Pausa la partida y abre el menú de opciones |

---

## ✨ Características Implementadas

### 🔹 Requerimientos Básicos
- **2 Jugadores:** Movimiento vertical independiente para Jugador 1 (`W`/`S`) y Jugador 2 (`Flechas`).
- **Sprites Personalizados:** Cada jugador cuenta con su imagen/sprite diferenciado (`Dragon.png` y `Rockon.png`).
- **Uso de Delta Time:** Todo movimiento y aplicación de fuerzas físicas se encuentra multiplicado por `Time.fixedDeltaTime` / `Time.deltaTime`.
- **Límites de Cancha:** Restricción de movimiento de las paletas dentro del área de juego mediante `Mathf.Clamp` y colisiones físicas.
- **Movimiento de Pelota:** Inicio en el centro, lanzamiento con ángulo aleatorio hacia uno de los lados y rebotes contra paredes y paletas.
- **Flujo de Menús:**
  - **Main Menu:** Inicio con pantalla de bienvenida, opciones, créditos y salida.
  - **Pause Menu:** Congelamiento del tiempo (`Time.timeScale = 0`) al pulsar `Escape` y retorno limpio entre menús.
- **Menú de Opciones en Pausa:**
  - Configuración de la **velocidad** de cada jugador en tiempo real mediante sliders.
  - Configuración del **alto de la paleta** (escala Y) de cada jugador mediante sliders.
  - Configuración del **color** de cada jugador (mediante paleta y botones de ciclado).

### 🚀 Requerimientos Avanzados
- **Movimiento 100% basado en Físicas:**
  - Las paletas se mueven aplicando fuerzas físicas (`Rigidbody2D.AddForce` en `FixedUpdate`).
  - La pelota se lanza y rebota utilizando impulsos físicos (`ForceMode2D.Impulse`) y físicas de colisión 2D.
- **Aceleración de la Pelota por Impacto:** Con cada choque contra las paletas de los jugadores, la velocidad de la pelota se incrementa progresivamente por un factor configurable (`speedIncreasePerHit`).
- **Rebote Angular Dinámico:** El ángulo de salida de la pelota varía de forma realista según la zona de la paleta donde impacte (arriba, centro o abajo).
- **Legibilidad y Calidad de Código:** Estándares de nomenclatura de C#, encapsulamiento (`[SerializeField] private`), comentarios explicativos y separación de responsabilidades.

---

## 📁 Estructura de Scripts (`Assets/TP02_C1_Scripts`)

| Script | Descripción |
| :--- | :--- |
| [`PaddleController.cs`](Assets/TP02_C1_Scripts/PaddleController.cs) | Control físico de las paletas (`AddForce`), límites de cancha y métodos para modificar velocidad, tamaño y color. |
| [`BallController.cs`](Assets/TP02_C1_Scripts/BallController.cs) | Lanzamiento inicial, aceleración por impacto con paletas, rebote angular y reinicio de la pelota. |
| [`GoalTrigger.cs`](Assets/TP02_C1_Scripts/GoalTrigger.cs) | Detecta la salida de la pelota por los laterales y ejecuta el reinicio de punto. |
| [`MainMenu.cs`](Assets/TP02_C1_Scripts/MainMenu.cs) | Gestión de pantalla de inicio, inicio de partida (`IsGamePlaying`) y lanzamiento de la pelota al pulsar *Play*. |
| [`PauseMenu.cs`](Assets/TP02_C1_Scripts/PauseMenu.cs) | Gestión de pausa con `Escape`, detención de físicas y sincronización con submenús. |
| [`SettingsMenu.cs`](Assets/TP02_C1_Scripts/SettingsMenu.cs) | Menú de configuración interactivo con auto-vinculación de referencias por código para velocidad, altura y colores. |

---

## ⚙️ Requisitos de Ejecución
- **Motor:** Unity 6 / Unity 2022.3 LTS o superior.
- **Pipeline:** Universal Render Pipeline (URP) o Built-in Render Pipeline.
- **Resolución sugerida:** 1920x1080 (16:9).
