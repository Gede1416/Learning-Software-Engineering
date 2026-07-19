---
name: sea-shader-study-plan
description: 用户正在学习 Seascape (sea.glsl) 海面shader，制定了5阶段16天学习计划
metadata: 
  node_type: memory
  type: project
  originSessionId: 34a51d04-fbd7-4f14-8956-3767529eacd0
---

用户当前学习目标：从基础光照模型知识出发，彻底理解 Shadertoy 上的 Seascape 海面 shader（sea.glsl），并能独立修改和扩展。

已完成：
- 已输出 sea_shader解析.md（完整代码解析）
- 已输出 sea_shader学习计划.md（5阶段16天学习计划）

学习计划分为五个阶段：
1. 噪声基础（hash + value noise）— 第1-3天 ✓ 已完成
2. FBM分形叠加（sea_octave + map()的循环）— 第3-5天 ✓ 已完成（2.4 octave_m跳过）
3. 光线步进（heightMapTracing的二分查找、相机系统）— 第6-9天 ✓ 已完成
4. 海洋着色（数值法线、菲涅尔、反射/折射、高光）— 第10-13天 ✓ 已完成
5. 整合与扩展（手写简化版、创意修改）— 第14-16天 🔄 进行中

当前进度：阶段五进行中 — 5.1完成 ✓，5.2进行中（数值法线待加）

阶段一完成内容（2026-06-28之前）：
- 1.1 hash 函数（dot压缩 + cos打乱 + fract取小数）
- 1.2 smoothstep 平滑（f*f*(3-2f)消除格子边界）
- 1.3 value noise（四角双线性插值 + 映射到[-1,1]）
- 1.4 频率概念（noise(uv*N)中N控制细节密度）
- 代码文件：_1_1hash.glsl, _1_2_smoothstep.glsl, 1_2_my_smoothsetp.glsl, _1_3_noise.glsl

阶段二完成内容：
- 2.1 FBM基础（多层noise叠加：freq*=2.0, amp*=0.5）
- 2.2 sea_octave（噪声扰动+正弦骨架+mix削尖+pow(choppy)尖峰化）
  - pow(1.0-pow(wv.x*wv.y,0.65),choppy)四层含义：二维波峰检测→变宽→翻转→压尖
- 2.3 正反向波（uv+time正向 + uv-time反向 → 碰撞感和驻波）
- 2.4 octave_m旋转矩阵（暂跳过）
- 代码文件：_2_1_fbm.glsl, _2_1_fbm_my.glsl, _2_2_sea_octave.glsl, _2_2_sea_octave_my.glsl, _2_3_waves.glsl

阶段三完成内容（2026-06-28至2026-06-30）：
- 3.1 射线生成（屏幕UV→NDC→3D视线方向，ori.z=-5是位置，dir.z=-2是焦距）
- 3.2 高度场几何（map(p)=p.y-fbm(p.xz)，>0天空/<0水中/=0海面）
- 3.3 二分查找数学（hm/(hm-hx)高度比加速，本质是试位法Regula Falsi非数组二分）
  - 重要认知：算法不保证找到第一个交点，是工程近似（高度场无悬挑+视线朝下+误差不可见）
- 3.4 对比实验（纯二分 vs 高度比，已在3.3中包含）
- 3.5 fromEuler相机旋转（ang.x=roll绕Z, ang.y=pitch绕X, ang.z=yaw绕Y；Rz·Rx·Ry手动展开）
- 代码文件：_3_1_ray_generation.glsl, _3_2_heightfield.glsl, _3_3_binary_search.glsl, _3_5_camera_rotation.glsl

阶段四完成内容（2026-07-01至2026-07-02）：
- 4.1 数值法线（getNormal中的梯度计算、map_detailed vs map、自适应eps）
- 4.2 菲涅尔效应（fresnel = 1-dot(n,-eye)、pow 3艺术调整、min 0.5钳制）
- 4.3 反射+折射（reflect(eye,n)→天空色、SEA_BASE+diffuse→水色、fresnel混合）
- 4.4 深度染色（p.y-SEA_HEIGHT波谷加深、atten大气散射衰减）
- 4.5 高光（Blinn-Phong specular、600极高指数、inversesqrt距离柔化）
- 代码文件：_4_1_normal.glsl, _4_2_fresnel.glsl, _4_3_reflection_refraction.glsl, _4_4_depth.glsl, _4_5_specular.glsl

阶段五进行中（2026-07-03至2026-07-06）：
- 5.1 手写简化版完成 ✓：用户独立实现 sea_octave + 高度比二分 + 固定法线 + 漫反射+高光
  - 关键理解：sv=1-abs(sin)检测过零点→1-翻转检测波峰→pow压尖；uv.x*=0.75产生方向性
  - sea_octave四步：噪声扰动→sin骨架→mix削尖→1-pow(乘积,0.65)翻转为波峰→pow(choppy)压尖
  - 代码文件：_5_1_simple_ocean.glsl（参考版）, _5_1_simple_ocean_my.glsl（用户手写版）
- 5.2 逐步加入完整功能 🔄：下一步加数值法线
- 学习笔记：0702_阶段五_整合与扩展.md

同时存在的新学习计划：
- ToonShader学习计划（shader/ToonShader学习计划.md）：Unity URP Toon Lit Shader，14天4阶段计划
- ToonShader阶段一笔记：1-学习笔记/ToonShader/0630_阶段一_URP架构.md
- ToonShader源码：UnityURPToonLitShaderExample/

已发现并修复的 bug：
- 1_2_my_smoothsetp.glsl 中 smoothstep 公式写成 l*l*(3.0-2.0)*l，漏了括号里的 *l
- 宏判断 #ifdef USE_SMOOTH > 1 应改为 #if USE_SMOOTH > 1

用户已知知识：基础光照模型（漫反射Lambert、高光Blinn-Phong），但对shader中的噪声函数、FBM、光线步进等技巧是初学者。

**Why:** 用户想系统学习这个shader而非只是看一遍，需要保留学习路径供后续参考。

**How to apply:** 当用户后续回来讨论这个shader的学习进度时，参考此计划给出建议。用户可能切换学习项目（Seascape ↔ ToonShader），需要支持进度恢复。
