# LowpolyOcean

Version : 1.2

Unity Version : 2018.3.11f

_LowpolyOcean_ is a highly customizable water shader system, style is _Low poly_.

Supports water surface and under water effects, and under water effects can be seamlessly switches.

Here is the built-in render pipeline version, The Lightweight RP version in [Unity asset store](https://assetstore.unity.com/packages/slug/134311).

Wiki : https://github.com/JiongXiaGu/LowpolyOcean/wiki

### Demo  (Note the Version)
- [GoogleDrive](https://drive.google.com/drive/folders/1velKf2LdrW4I9dhpaCaa2XS_lD5siBMy)
- [BaiduPan](https://pan.baidu.com/s/1i-1KVi470Ro2tEPD6TGuEQ)

### Comparison

All code has been rewritten and optimized in the Lightweight RP version.

| Mode | built-in RP | Lightweight RP |
| :--: | :--: | :--: |
| Performance | Slower | Faster |
| Lighting style | Pixel | Pixel, Flat |
| Fron Side Lighting Module | UnityPBS | Blinn-Phong, SunShine |
| Point Light | Built-in method | Blinn-Phong |
| Reflection | Planar | Color, CubeTexture, Probes |
| Under Water | Simple | Advanced |

### Known problem
* Refraction consumes too much performance, unless the water does not receive shadows, In Lightweight RP, transparent objects can also receive shadows (limited), and opaque texture is relatively cheap to obtain.
* In order to achieve refraction offset effect, need to use camera to render the camera depth texture of water. In Lightweight RP, only one pass is needed.
* In Unity 2019.1.0f2, shadow display is incorrect.

### Thanks

Unity Document : https://docs.unity3d.com/Manual/index.html

GPU Gems : https://developer.nvidia.com/gpugems/GPUGems/gpugems_pref01.html

Catlike Coding : https://catlikecoding.com/

Assassin’s Creed III: The tech behind (or beneath) the action : https://www.fxguide.com/featured/assassins-creed-iii-the-tech-behind-or-beneath-the-action/

Reference for HLSL : https://docs.microsoft.com/zh-cn/windows/desktop/direct3dhlsl/dx-graphics-hlsl

Assassin’s Creed: Black Flag – Waterplane : https://simonschreibt.de/gat/black-flag-waterplane/

### Effect

![1](https://github.com/JiongXiaGu/LowpolyOcean/blob/master/Image/FaFaFa.png)
![2](https://github.com/JiongXiaGu/LowpolyOcean/blob/master/Image/636799873936840380.png)
