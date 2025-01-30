# Maze of Legends

## Descripción
**Maze of Legends** es un emocionante juego de laberinto diseñado para dos jugadores, donde la estrategia y la habilidad son clave para la victoria. 
En **Maze of Legends**, los jugadores se sumergen en un desafiante laberinto en el que deben recolectar **tres frutas** antes que su oponente. Cada jugador elige un campeón de las icónicas regiones de **Demacia** y **Noxus**, enfrentándose a enemigos naturales y utilizando sus habilidades para superar los obstáculos del laberinto.
El objetivo es simple: recolectar tres frutas antes que tu oponente mientras navegas por un laberinto lleno de desafíos. Usa tus habilidades sabiamente para desactivar trampas y superar a tus enemigos en esta intensa carrera por la victoria.

## Instalación

1. **Requisitos previos**: Asegúrate de tener [.NET SDK](https://dotnet.microsoft.com/download) instalado en su dispositivo. 
3. **Clonar repositorio**:
   ```bash
   git clone https://github.com/soft-bubble/Maze-of-Legends-Console-Ver-.git
   cd Maze-of-Legends-Console-Ver-
   ```
4. **Build repositorio**:
   ```bash
   dotnet build
   ```
5. **Ejecutar el juego**:
   ```bash
   dotnet run
   ```
> **Nota**: Para que el visual funcione correctamente, asegúrate de tener instaladas las bibliotecas **Console.Spectre** y **NAudio**.

## ¿Cómo jugar Maze of Legends? Nociones.
Al iniciar **Maze of Legends**, los jugadores se encuentran con una interfaz intuitiva que ofrece cuatro opciones claras:

1. **Start**: Inicia el juego y te lleva a la pantalla de selección de personaje del primer jugador.
2. **Credits**: Accede a la pantalla de créditos, donde se reconoce el esfuerzo y la colaboración de todos los involucrados en el proyecto.
3. **Controls**: Muestra los controles del juego, junto con una leyenda de cada símbolo utilizado en el laberinto. También incluye una explicación detallada de las habilidades de los campeones, las trampas presentes y las condiciones necesarias para alcanzar la victoria.
4. **Exit**: Cierra el juego y te regresa al escritorio.
<div align="center">
<img src="https://github.com/user-attachments/assets/dd78a599-e9df-48bf-a04c-90ea6ecbf322" alt="{39791602-5C5D-42C6-B4C8-7A1BDA3D2015}" width="300"/>
</div>

Al presionar **Start** con la tecla *Enter*, se accede a la **Pantalla de Selección de Personaje**. En esta etapa, los jugadores pueden elegir sus campeones, utilizando las flechas *Up* y *Down* de su teclado, de la siguiente manera:

1. **Selección del Primer Jugador**: 
   - El primer jugador elige su campeón de entre cinco opciones, cada una representando a un icónico héroe de la región correspondiente.
   - Una vez realizada la selección, el jugador debe presionar *Enter* para confirmar su elección.

2. **Selección del Segundo Jugador**: 
   - Después de que el primer jugador haya elegido, es el turno del segundo jugador para seleccionar su campeón.
   - Al igual que el primer jugador, el segundo jugador tiene cinco opciones disponibles, todas pertenecientes a la misma región.

> **Nota**: Recuerda que cada jugador puede elegir *solo* un campeón.

Se visualiza de la siguiente forma:
<div align="center">
<img src="https://github.com/user-attachments/assets/53d2c6e5-da3c-40bf-a8e6-efc2e73942ef" alt="{84624AEB-8CAB-4E27-9E1B-E11FC66F1342}" width="300"/>
<img src="https://github.com/user-attachments/assets/48626609-a859-4b6f-b96f-f8f7976e8bf1" alt="{8D4A697F-F235-41B2-8D4B-B3EEFF47AC1F}" width="300"/>
</div>

Una vez que los campeones han sido seleccionados, los jugadores deben elegir la dificultad del laberinto de entre **10 opciones** disponibles. Cada nivel de dificultad presenta un rango de laberintos diferente, lo que afecta directamente la experiencia de juego:

- **Rangos de Dificultad**: 
  - Las dificultades más bajas ofrecen laberintos más simples y menos desafiantes.
  - A partir del rango **Esmeralda**, la complejidad del laberinto aumenta significativamente, introduciendo un mayor número de trampas y obstáculos que los jugadores deberán sortear.

Esta elección permite a los jugadores ajustar el desafío según su nivel de habilidad y experiencia, asegurando que cada partida sea emocionante y única.

Se visualiza de la siguiente forma:
<div align="center">
<img src="https://github.com/user-attachments/assets/7a8ce81a-536b-43f4-a69e-84129e0526b5" alt="{E441DFD6-8E46-4DB3-AA51-69102CB64DC4}" width="200"/>
</div>

Una vez seleccionada la dificultad, el juego da comienzo. En la pantalla aparecerán **tres paneles** y un laberinto interactivo. A continuación, se describen los componentes de la interfaz:

1. **Panel de Contador de Frutas**:
   - Este panel muestra la cantidad de frutas en posesión del jugador. 
   - El objetivo es alcanzar **3 frutas** para ganar el juego.

  Imagen de referencia:
<div align="center">
<img src="https://github.com/user-attachments/assets/534de6ed-2038-4c9f-b0f9-092416195d80" alt="{D725A125-C43B-4140-8EBA-D63193B27358}" width="200"/>
</div>

2. **Panel de Enfriamientos**:
   - Situado sobre el laberinto, este panel proporciona información sobre los enfriamientos de la trampa **Root** (que será explicada más adelante), así como el enfriamiento de la habilidad principal y secundaria del campeón.

    Imagen de referencia:
<div align="center">
<img src="https://github.com/user-attachments/assets/41079bfd-db8a-440b-9445-c9a5e0efc02a" alt="{5957CB74-CB48-446A-A91A-BF34B301B11C}" width="400"/>
</div>

4. **Panel de Información del Campeón**:
   - Ubicado debajo del laberinto, este panel contiene información crucial sobre el estado del campeón en turno:
     - **Nombre del Campeón**: Indica qué personaje está activo.
     - **Habilidad Principal**: Muestra la disponibilidad de la habilidad principal.
     - **Habilidad Secundaria**: Indica la disponibilidad de la habilidad secundaria.
     - **Velocidad**: Muestra la velocidad disponible durante el turno.
     - **Estado de Trampa**: Indica si el campeón está bajo el efecto de la trampa **Root** (Cursed).
     - **Posición Actual**: Muestra las coordenadas (x, y) del campeón en el laberinto.

Esta estructura de paneles proporciona a los jugadores la información necesaria para tomar decisiones estratégicas a lo largo del juego.

   Imagen de referencia:
<div align="center">
<img src="https://github.com/user-attachments/assets/2e8a4527-33aa-4b57-b404-515f8938317c" alt="{4B40EA11-575A-439D-8E30-C6468645654F}" width="400"/>
</div>

## Controles y visual del laberinto.

El laberinto generado, sin importar su tamaño, presenta una serie de **símbolos visuales** que son cruciales para la experiencia de juego. Es fundamental que los jugadores reconozcan el significado de cada símbolo para disfrutar plenamente del juego y tomar decisiones informadas.

   Imagen de referencia:
<div align="center">
<img src="https://github.com/user-attachments/assets/aea2dad5-8b47-4194-83c7-1d4a1e3cef86" alt="image" width="200"/>
</div>

**Leyenda**: 
1. Pared:  <img src="https://github.com/user-attachments/assets/3cf770d4-ef57-4fb9-8138-fde593858646" alt="{DCF15D3B-FDE1-489C-A243-162DAB41F1EF}" width="20"/>
2. Camino:  <img src="https://github.com/user-attachments/assets/16e6b37d-1277-4c4d-91fb-71588450a1a6" alt="{A0C57B8E-4625-4B9B-AEC5-9D47FAFF7CCB}" width="20"/>
3. Trampa:  <img src="https://github.com/user-attachments/assets/d34f4919-a9c4-4c82-a4cf-69cb97d78aec" alt="{E0E1F5A7-0594-49E2-AF61-829CA3B52B25}" width="20"/>
4. Obstáculo:  <img src="https://github.com/user-attachments/assets/68d732b9-bb08-4fe3-9d36-19f7db0d1958" alt="{1C280885-65AE-4E3F-A56A-E650A4BBE33E}" width="20"/>
5. Fruta:  <img src="https://github.com/user-attachments/assets/24b6696b-16eb-420a-8caf-0c66f2fcaf99" alt="{1B2F14CE-3E66-49AF-AC11-D2CE51B0B644}" width="20"/>
6. Campeón de Demacia: <img src="https://github.com/user-attachments/assets/d45c6766-5de0-4b86-be8b-88105f2083e2" alt="{3D99F9E7-EA6A-415E-B0AE-677ECAE0813A}" width="20"/>
7. Campeón de Noxus: <img src="https://github.com/user-attachments/assets/c257f6e9-ce56-4fa1-8727-ac2802fdf321" alt="{791DE1C5-42C2-4DAE-BA93-E56F5CD8511F}" width="20"/>







## Contact
If you have questions or suggestions, feel free to contact me:
- **Email**: softbubbleerg@gmail.com
- **GitHub**: soft-bubble (https://github.com/soft-bubble)

