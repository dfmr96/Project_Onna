# **📑 Sistema de Mutaciones – ONИA**

**Versión:** v0.2.0  
 **Fecha:** Mar 29, 2025

---

## **📄 Descripción General**

Las **mutaciones** son mejoras temporales que el jugador obtiene al completar cada sala o nivel.

* Solo duran durante la **partida actual (run)**.

* Al finalizar la run, ya sea ganando o perdiendo, se eliminan.

* Funcionan mediante la asignación de **radiaciones** en distintos **sistemas corporales** del personaje.

---

## **🎯 Objetivos**

* Ofrecer variedad estratégica en cada partida, generando builds únicas.

* Reforzar la progresión temporal del jugador dentro de una run.

* Generar sinergias entre radiaciones, sistemas y estilo de juego.

* Mantener rejugabilidad mediante elecciones aleatorias.

---

## **✅ Criterios de Éxito**

* El jugador recibe siempre **3 opciones aleatorias de radiaciones** al completar una sala.

* Cada elección tiene **impacto inmediato y reconocible** en el estilo de juego.

* El sistema escala con **mejoras pasivas** (hasta nivel 4\) si se repiten elecciones.

* La combinación de radiaciones \+ sistema produce efectos diferenciados y claros.

---

## **⚙️ Mecánicas y Componentes**

| Componente | Descripción |
| ----- | ----- |
| **Sistemas corporales** | Nervioso, Tegumentario y Muscular. Cada uno tiene 2 slots: una Mutación Mayor y una Mutación Menor. |
| **Radiaciones** | Gamma, Alfa, Beta, Neutrones, **Impulso Temporal**, Cherenkov. |
| **Mutación Mayor** | Primera radiación en un sistema vacío. Efectos potentes y definitorios. |
| **Mutación Menor** | Segunda radiación distinta en un sistema. Efectos secundarios o de apoyo. |
| **Mejora pasiva** | Radiación repetida en un sistema con slots llenos. Incrementa potencia (+1 nivel, máx. 4). |
| **Selección** | Tras cada sala, aparecen 3 radiaciones aleatorias. El jugador elige una y la asigna a un sistema. |

---

## **🔬 Mutaciones Mayores y Menores**

### **🧠 Sistema Nervioso**

* **Gamma**

  * Mayor: Aumenta la cantidad sanada por los orbes vitales, pero incrementa el consumo de vida por segundo.

  * Menor: Atrae de más lejos los orbes de curación.

* **Alfa**

  * Mayor: Cada orbe recogido otorga un breve periodo de invulnerabilidad (con cooldown).

  * Menor: Cada orbe da un mini-burst de invulnerabilidad muy breve (sin cooldown).

* **Beta**

  * Mayor: Aumenta la velocidad de movimiento y dash.

  * Menor: Incrementa ligeramente la velocidad de movimiento.

* **Neutrones (Tiempo Vital)**

  * Mayor: Cada orbe vital recogido añade \+5s de tiempo vital (máx. 120s).

  * Menor: Cada orbe vital recogido añade \+2s.

* **Microondas (Quemaduras)**

  * Mayor: Disparos aplican quemadura de 3s. Si el enemigo ya está quemado, el disparo hace más daño inicial.

  * Menor: Disparos aplican quemadura leve de 1–2s.

* **Cherenkov**

  * Mayor: Los enemigos marcados sueltan orbes adicionales al morir.

  * Menor: Enemigos marcados sueltan 1 orbe extra en vez de varios.

    ---

    ### **🛡️ Sistema Tegumentario**

* **Gamma**

  * Mayor: Aura radial de alta energía que inflige daño continuo a enemigos cercanos.

  * Menor: Aura radial más débil, se activa cada X segundos (no constante).

* **Alfa**

  * Mayor: Al recibir golpes, libera una onda corta que inflige daño elevado a enemigos cercanos.

  * Menor: Al recibir daño, libera onda corta que empuja a los enemigos.

* **Beta**

  * Mayor: Campo de fricción que reduce la velocidad de enemigos cercanos.

  * Menor: Reduce la velocidad de algunos enemigos cercanos (efecto aleatorio).

* **Neutrones (Tiempo Vital)**

  * Mayor: Al recibir daño, existe un 20% de probabilidad de recuperar \+2s de tiempo vital.

  * Menor: Activar el campo ralentiza el consumo de tiempo vital durante 3s.

* **Microondas (Quemaduras)**

  * Mayor: Campo térmico alrededor del jugador que aplica quemadura de 3s.

  * Menor: Campo reducido que aplica quemadura breve (1–2s).

* **Cherenkov**

  * Mayor: Área que debilita a los enemigos cercanos, haciéndolos recibir más daño.

  * Menor: Área que debilita enemigos cercanos, haciendo que realicen menos daño.

    ---

    ### **💪 Sistema Muscular**

* **Gamma**

  * Mayor: Disparos con gran penetración que atraviesan múltiples enemigos.

  * Menor: Disparos atraviesan obstáculos del escenario, pero no enemigos adicionales.

* **Alfa**

  * Mayor: Disparos de alto daño, pero con cadencia más lenta.

  * Menor: Aumenta el daño del disparo, pero limita la penetración a 2 objetivos.

* **Beta**

  * Mayor: Disparos rápidos que aplican ralentización a los enemigos.

  * Menor: Disparos aplican ralentización básica.

* **Neutrones (Tiempo Vital)**

  * Mayor: Cada 10 enemigos eliminados otorgan \+10s de tiempo vital (máx. 120s).

  * Menor: Cada 15 disparos acertados sin fallar devuelven \+3s de tiempo vital.

* **Microondas (Quemaduras)**

  * Mayor: Disparos aplican quemadura de 3s.

  * Menor: Disparos aplican quemadura corta (1–2s).

* **Cherenkov**

  * Mayor: Disparos que marcan a los enemigos, aumentando el daño que reciben de todas las fuentes.

  * Menor: Disparos marcan enemigos, solo aumentan un poco el daño recibido.

    ---

    ### **⚔️ Melee (Secundario)**

* **Gamma**

  * Mayor: El combo melee emite una onda alrededor del jugador que daña en área y prolonga la inmovilización inicial.

  * Menor: El segundo golpe del combo libera un pulso que ralentiza brevemente.

* **Alfa**

  * Mayor: Al recibir golpes carga el arma, haciendo que el siguiente ataque inflija más daño y empuje.

  * Menor: Golpear con melee empuja a los enemigos.

* **Beta**

  * Mayor: Los golpes melee aplican ralentización fuerte durante un instante.

  * Menor: El primer golpe genera un campo de fricción mínimo que reduce velocidad.

* **Neutrones (Tiempo Vital)**

  * Mayor: Cada enemigo derrotado con melee devuelve \+1s de tiempo vital.

  * Menor: El segundo golpe del combo ralentiza el consumo de tiempo vital durante 2s.

* **Microondas (Quemaduras)**

  * Mayor: Cada golpe del combo aplica quemadura de 3s. Si el enemigo ya estaba quemado, aumenta la inmovilización.

  * Menor: Golpes aplican quemadura leve de 1s.

* **Cherenkov**

  * Mayor: Melee marca enemigos, haciéndolos recibir más daño.

  * Menor: Enemigos marcados infligen menos daño por unos segundos.

    ---

    ### **💣 Sistema Endocrino (Granadas)**

* **Gamma**

  * Mayor: La explosión genera un campo radiactivo persistente que daña continuamente.

  * Menor: La explosión deja un rastro radiactivo breve.

* **Alfa**

  * Mayor: La granada inflige daño inicial mucho mayor, pero con radio más pequeño.

  * Menor: La explosión empuja a los enemigos.

* **Beta**

  * Mayor: La explosión crea un campo de fricción que ralentiza a todos los enemigos.

  * Menor: Los enemigos alcanzados sufren ralentización leve.

* **Neutrones (Tiempo Vital)**

  * Mayor: Cada enemigo alcanzado por la granada devuelve \+1s de tiempo vital (máx. \+5s).

  * Menor: Si la granada impacta directo, devuelve \+2s de tiempo vital.

* **Microondas (Quemaduras)**

  * Mayor: La explosión deja un campo ardiente que aplica quemadura de 3s.

  * Menor: La explosión aplica quemadura básica de 2s a todos los alcanzados.

* **Cherenkov**

  * Mayor: La explosión marca a todos los enemigos alcanzados, haciéndolos recibir más daño.

  * Menor: Enemigos alcanzados infligen menos daño mientras dure la marca.

---

## **🔩 Lógica**

1. El jugador completa una sala/nivel.

2. Se generan **3 radiaciones aleatorias**.

3. El jugador elige una radiación:

   * Sistema vacío → **Mutación Mayor**.

   * Sistema con Mayor → **Mutación Menor**.

   * Sistema con ambas y radiación repetida → **Mejora pasiva** (+1 nivel, máx. 4).

4. El proceso se repite hasta finalizar la run.

---

## **🔧 Configuración del Sistema**

| Variable | Valor |
| ----- | ----- |
| **NumSistemas** | 3 (Nervioso, Tegumentario, Muscular) |
| **Slots por sistema** | 2 (Mayor \+ Menor) |
| **Radiaciones disponibles** | 6 (Gamma, Alfa, Beta, Neutrones, Microondas, Cherenkov) |
| **Opciones por sala** | 3 aleatorias |
| **Nivel máximo de mejora pasiva** | 4 |
| **Tiempo vital base** | 60s (máx. 120s) |

