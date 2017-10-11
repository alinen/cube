# cube
Rubik's cube trainer

![animation](https://github.com/alinen/cube/raw/gh-pages/Docs/rubiks2.gif "A fun pattern")

# Solving
The cube solution algorithm is based on the book Mastering the Cube by Don Taylor. A high level description of the algorithm (with animations) is [here](https://alinen.github.io/cube)

# Implementation
This project is written C# for [Unity](https://unity3d.com). 
* Assets/Cube/solve.unity - scene for self-solving cube
* Assets/Cube/demo.unity - scene for playing a cube animation based on F, B, D, L, R, U notation. Note that notation is absolute (in terms of X, Y, Z axes) as opposed to relative to the player. Future work will add relative commands.
