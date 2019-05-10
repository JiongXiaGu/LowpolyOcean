# LowpolyOcean
Unity Version : 2018.3.11f

_LowpolyOcean_ is a highly customizable water shader system, style is _Low poly_.

Supports water surface and under water effects, and under water effects can be seamlessly switches.

Here is the built-in render pipeline version, The Lightweight RP version in [Unity asset store](https://assetstore.unity.com/packages/slug/134311).

### Comparison

| Mode | built-in RP | Lightweight RP |
| :--: | :--: | :--: |
| Lighting style | Pixel | Pixel, Flat |
| Fron Side Lighting Module | UnityPBS | Blinn-Phong, SunShine |
| Point Light | Built-in method | Blinn-Phong |
| Reflection | Planar | Color, CubeTexture, Probes |
| Performance | Slower | Fast |

### Known problem
* Refraction consumes too much performance, unless the water does not receive shadows, In Lightweight RP, transparent objects can also receive shadows (limited), and opaque texture is relatively cheap to obtain.
* In order to achieve refraction offset effect, need to use camera to render the camera depth texture of water. In Lightweight RP, only one pass is needed.
* 

### Effect

![1](https://github.com/JiongXiaGu/LowpolyOcean/blob/master/Image/FaFaFa.png)
![2](https://github.com/JiongXiaGu/LowpolyOcean/blob/master/Image/636799873936840380.png)
