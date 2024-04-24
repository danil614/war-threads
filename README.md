# WarThreadsGUI

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)

## Description

This repository contains a GUI application developed in C# WinForms for a simple war-themed game. The game features various functionalities including the appearance of a large ship with significant life points (5% chance of appearance) and three types of enemies with different movement speeds.

#### Upon initialization, a cannon PictureBox object is created.
![image](https://github.com/danil614/war-threads/assets/71091627/7e7248b5-a59b-49d6-a417-1596a4bf3dcc)

#### The InitializeGame method initiates threads for enemy generation and speed incrementation over time.
![image](https://github.com/danil614/war-threads/assets/71091627/4f9e7bb2-6d0d-4e1b-a663-4b211c165242)

#### The GenerateEnemies method utilizes an event, waiting for 15 seconds for the user to press the left or right arrow keys. Subsequently, a separate thread is created for each enemy.
![image](https://github.com/danil614/war-threads/assets/71091627/77af1279-6f09-48e5-8c1b-f0b2b676e277)

#### In the CreateEnemy method, a 5% chance is implemented for the appearance of a large ship (enemy).
![image](https://github.com/danil614/war-threads/assets/71091627/d1d54955-fb94-45a9-8046-a299af627456)

#### Collision detection between enemies and bullets is also implemented here.
![image](https://github.com/danil614/war-threads/assets/71091627/42dcfe3b-2b21-4585-a85b-86bab3705fc0)
#### When a collision occurs, the enemy's life points are decremented, and if it reaches zero, the enemy is removed.

#### The HandleHit method utilizes thread-safe incrementation to count the number of hits.
![image](https://github.com/danil614/war-threads/assets/71091627/9ebfc206-1e9a-4914-bc95-6d240e61147d)

#### A semaphore is employed in the FireBullet method to limit the number of bullets, and it is released upon bullet deletion.
![image](https://github.com/danil614/war-threads/assets/71091627/ea76eaaa-7ca5-4d9a-a384-f9e179e74c94)
![image](https://github.com/danil614/war-threads/assets/71091627/63b894d5-d547-4c6d-bd6c-891a12468433)

The game concludes when the player reaches a certain number of misses.

## Screenshots of the game

### The start window
![image](https://github.com/danil614/war-threads/assets/71091627/77714083-0952-444f-b854-3f4bd7d51d2b)

### Running game
![image](https://github.com/danil614/war-threads/assets/71091627/2992422a-861d-427b-ab4a-57b65d1666c6)
![image](https://github.com/danil614/war-threads/assets/71091627/d3037a12-b11e-43f4-a8a5-e182b8a47d0a)

### A large ship appeared
![image](https://github.com/danil614/war-threads/assets/71091627/3fa85943-d352-45aa-9ba7-42d8ef933ca3)

### Loss
![image](https://github.com/danil614/war-threads/assets/71091627/124ae7cd-592f-4c18-95a7-ffe13be6d39d)
