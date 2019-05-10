
------------------------

JiongXiaGu.LowpolyOcean

Time : 20190418
Version 1.2
Unity Version : 2018.3.11f

Thank you for use Lowpolyocean!

Wiki : https://github.com/JiongXiaGu/LowpolyOcean/wiki
Update content :  https://github.com/JiongXiaGu/LowpolyOcean/wiki/Version_1.2

------------------------

<Getting Started>
	1.Need to provide a layer(default 24) for rendering clip mark texture and under ocean mark texure.
	2.Need to initialize "ProjectSettings" when the game is running or editor running, you can choose to throw the prefab "OceanController" into the scene, or initialize it in your code.
	3.The color space for all examples is work at "Linear", you can also set the same color space, for better result.(Color Space : https://docs.unity3d.com/Manual/LinearLighting.html)
	4.Code work at ".NET 4.x" (.NET profile support : https://docs.unity3d.com/Manual/dotnetProfileSupport.html)
	5.Add Component "OceanCameraTask" to the camera, most features don't support "MSAA", so you need to turn off the camera's "MSAA".

<Examples>
	1.All examples work at DX11, except for the name have "DX9" or "DX10".

<Wave>
	1.The waves are simulate by a texture.
	 The final height is determined by the color channel "rgb", the "a" channel determines the vertex horizontal offset. 
	2.Wave texture need to turn off the "sRGB" option.

<Refraction>
	1.if you need to turn on refraction effect, you should turn off the camera's "MSAA"
	2.There are two ways to get the refraction texture(these options can be set in "ProjectSettings"). 
	 (1)Use the camera directly, the rendering cost is increased according to the complexity of the scene.
	 (2)Get the texture through the cache, which requires the ocean rendering queue is "Transparent"(meaning the ocean in the existing rendering pipeline cannot receive shadows, and does not support the under ocean fog effect), relatively low cost;
	 
<Reflection>
	1.Refraction is recommended for use on less undulating ocean.

<Ocean Clip>
	1.This is a method of providing ocean cut, to cut ocean in a given area. more information in the example scene "BoatOceanClip".
	2.This function uses a lot of conditional statements in the shader.

<Ripple>
	1.Use textures to control ocean height, different from waves. see more information on script "RippleOptions.cs" and example scene "Ripple".
	2.Ripple texture need to turn off the "sRGB" option.

<UnderOcean>
	1.For objects that want to achieve under ocean effects, you need to use a special material. Here is a rewrite of the unity standard material. If you need to add under ocean effect to your shader script, see "LPUnderOceanLighting.cginc" and shader "ExampleDiffuse.shader"(LowpolyOcean/Examples/UnderOceanDiffuse)

<Buoyancy>
	1.The content of the namespace "JiongXiaGu.BuoyancySystems" may change in the future. If you want to achieve your own buoyancy, you can calculate the ocean height by "VertexHelper.GetWaveHeight()".