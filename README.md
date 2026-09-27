# 🏓 TP04: Dragon Pong 2D - Físicas 2D, ScriptableObjects y Obstáculos Dinámicos

Proyecto desarrollado en **Unity** correspondiente a la entrega del **Trabajo Práctico 04 (TP04)**. Esta entrega integra y amplía todas las mecánicas del TP03, incorporando **movimiento físico 2D con `AddForce`**, delimitación por mitad de cancha, **Scriptable Objects** para parametrización, **partidas competitivas al mejor de 5**, **temporizador de 20s con auto-gol** y un **sistema avanzado de obstáculos temporales** con variedad de ángulos y rebotes físicos.

El desarrollo sigue la filosofía **Ponytail** (código pragmático, modular, limpio y basado en el principio *YAGNI*).

---

## 🎮 Controles de Juego

| Jugador / Acción | Movimiento Vertical | Movimiento Horizontal | Delimitación de Cancha |
| :--- | :---: | :---: | :--- |
| **Jugador 1 (Izquierda)** | <kbd>W</kbd> / <kbd>S</kbd> | <kbd>A</kbd> / <kbd>D</kbd> | Entre su línea de arco ($X \approx -8.0$) y el mediocampo ($X \le -0.3$) |
| **Jugador 2 (Derecha)** | <kbd>▲</kbd> / <kbd>▼</kbd> | <kbd>◄</kbd> / <kbd>►</kbd> | Entre el mediocampo ($X \ge 0.3$) y su línea de arco ($X \approx 8.0$) |
| **Pausa / Opciones** | <kbd>Escape</kbd> | — | Pausa la partida y permite ajustar opciones o volver al menú |

---

## ✨ Características Implementadas (TP04)

### 🔹 1. Requerimientos Básicos

- **Mecánicas del TP03 integradas y conservadas:**
  - Sistema completo de menús: Menú Principal, Menú de Pausa, Panel de Opciones y Créditos.
  - Personalización en vivo de velocidades, altura de paleta y paleta de colores.
  - Gestión del tiempo con `Time.timeScale` y cálculo físico en `Time.fixedDeltaTime`.
- **Físicas 2D Puras (`Rigidbody2D` $\rightarrow$ `AddForce`):**
  - Todo movimiento de paletas, pelota y objetos en escena está controlado mediante fuerzas físicas en `FixedUpdate`.
  - Frenado suave y aceleración angular en los rebotes.
- **Movimiento 2D y Delimitación de Cancha:**
  - Ambas paletas pueden moverse tanto en el eje vertical como horizontal (<kbd>WASD</kbd> para P1 y <kbd>Flechas</kbd> para P2).
  - Cada jugador está estrictamente restringido a su respectiva mitad de cancha (no pueden invadir el campo rival ni salirse de los límites).
- **Scriptable Objects para Inicialización (`GameSettings`):**
  - Centraliza variables de configuración: goles para ganar, tiempo límite de posesión, velocidades base, multiplicador de aceleración y dimensiones de obstáculos.
  - Incluye fallback procedural por código si no se asigna un asset en el Inspector.
- **Partida al Mejor de 5 (Gana quien llega a 3 goles):**
  - Marcador en vivo gestionado por `GameManager`.
  - Al alcanzar los 3 goles (configurable en `GameSettings`), se declara al ganador, se despliega el panel de **Game Over** y se ofrecen botones para *Jugar de nuevo* o *Volver al Menú Principal*.
- **Límite de Tiempo de 20 Segundos por Jugada:**
  - Temporizador regresivo en pantalla con alerta visual en los últimos segundos.
  - **Regla de Auto-Gol:** Si se agotan los 20 segundos sin convertir, el sistema comprueba en qué mitad de cancha se encuentra la pelota ($X < 0$ o $X \ge 0$) y se le cobra gol en contra automáticamente al jugador de ese lado.
- **Reglas Dinámicas de Color de Paletas:**
  - **Choque con límites/paredes:** La paleta cambia automáticamente a color **Negro** (`Color.black`).
  - **Impacto con la pelota:** La paleta cambia automáticamente a un **color aleatorio** en cada golpe.
- **Aceleración de la Pelota por Impacto:**
  - La velocidad de la pelota aumenta progresivamente tras cada contacto con las paletas (`speedIncreasePerHit`) hasta alcanzar un límite máximo (`ballMaxSpeed`).
- **Calidad de Código y Estandarización:**
  - Documentación XML exhaustiva en clases y métodos.
  - Nomenclatura uniforme (camelCase en variables privadas y PascalCase en miembros públicos y clases).
  - Encapsulamiento y arquitectura desacoplada.

---

### 🚀 2. Requerimientos Avanzados

- **Sistema de Obstáculos Dinámicos Temporales:**
  - Spawner periódico (`ObstacleSpawner`) que genera obstáculos en posiciones aleatorias de la zona central de la cancha.
- **Tiempo de Vida Aleatorio (3 a 7 segundos):**
  - Cada obstáculo se destruye automáticamente tras un intervalo aleatorio entre **3 y 7 segundos** (`Random.Range(3f, 7f)`).
  - Incluye una transición suave de desvanecimiento (*fade-out*) en sus últimos 0.5 segundos de vida.
- **Variedad de Ángulos de Inclinación:**
  - Los obstáculos aparecen con rotación angular aleatoria (entre **-75° y +75°**), generando rebotes impredecibles y dinámicos para la pelota.
- **Físicas y Colisiones Elásticas:**
  - Equipados con `PhysicsMaterial2D` de rebote perfecto (`bounciness = 1`, `friction = 0`).
  - Soporte para asignación directa de **Sprite**, **Prefab personalizado** o generación procedural con dimensiones compactas configurables.

---

## 📁 Estructura de Scripts (`Assets/TP02_C1_Scripts`)

| Script | Rol / Responsabilidad |
| :--- | :--- |
| [`GameSettings.cs`](Assets/TP02_C1_Scripts/GameSettings.cs) | **ScriptableObject**: Centraliza goles para ganar (3), tiempo límite (20s), velocidades y parámetros de obstáculos. |
| [`GameManager.cs`](Assets/TP02_C1_Scripts/GameManager.cs) | **Singleton / Game Loop**: Controla el marcador, temporizador de 20s, cobro de auto-gol por lado de cancha, pantalla de Game Over y reinicio. |
| [`PaddleController.cs`](Assets/TP02_C1_Scripts/PaddleController.cs) | Movimiento físico 2D con `AddForce` (WASD / Flechas), límites por mitad de cancha y cambios de color (Negro con límites, aleatorio con pelota). |
| [`BallController.cs`](Assets/TP02_C1_Scripts/BallController.cs) | Lanzamiento físico con `AddForce`, aceleración por impacto, rebote angular dinámico y sincronización con `GameManager`. |
| [`GoalTrigger.cs`](Assets/TP02_C1_Scripts/GoalTrigger.cs) | Detecta cuando la pelota cruza el arco y suma el punto al rival a través de `GameManager`. |
| [`Obstacle.cs`](Assets/TP02_C1_Scripts/Obstacle.cs) | Comportamiento del obstáculo: tiempo de vida de 3 a 7 segundos, colisionador 2D y efecto visual de fade-out. |
| [`ObstacleSpawner.cs`](Assets/TP02_C1_Scripts/ObstacleSpawner.cs) | Generador de obstáculos en el centro de la cancha, con rotación angular aleatoria, soporte para sprites/prefabs y limpieza por ronda. |
| [`MainMenu.cs`](Assets/TP02_C1_Scripts/MainMenu.cs) | Navegación del Menú Principal, inicio de partida y reinicio de tanteador. |
| [`PauseMenu.cs`](Assets/TP02_C1_Scripts/PauseMenu.cs) | Menú de Pausa (<kbd>Escape</kbd>), detención del tiempo y retorno al Menú Principal o ajustes. |
| [`SettingsMenu.cs`](Assets/TP02_C1_Scripts/SettingsMenu.cs) | Menú interactivo para cambiar velocidad, altura de paleta y colores de los jugadores en tiempo real. |

---

## 🛠️ Configuración y Ejecución en Unity

1. **Abrir el proyecto en Unity** (Unity 6 / Unity 2022.3 LTS o superior).
2. Abrir la escena principal en `Assets/Scenes/SampleScene.unity`.
3. **Crear o editar `GameSettings` (Opcional):**
   - En la carpeta de Project: Clic derecho $\rightarrow$ `Create` $\rightarrow$ `DragonPong` $\rightarrow$ `GameSettings`.
   - Ajustar `Points To Win = 3` y `Round Time Limit = 20`.
4. **Verificar componentes en escena:**
   - Objeto `GameManager` con el script `GameManager.cs`.
   - Objeto `ObstacleSpawner` con el script `ObstacleSpawner.cs`.
5. Presionar **Play** ▶️ para jugar.
