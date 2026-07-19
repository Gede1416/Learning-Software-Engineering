---
name: toon-shader-study-plan
description: 用户正在学习 Unity URP Toon Lit Shader（NiloCat示例），4阶段14天学习计划
metadata: 
  node_type: memory
  type: project
  originSessionId: f8440ebb-7d18-41d8-83d3-426f46a18c55
---

用户当前学习目标：从 URP Shader 基础架构出发，彻底理解 ColinLeung-NiloCat 的 Unity URP Toon Lit Outline Shader，并能独立修改光照公式、描边效果和扩展新功能。

学习计划分为四个阶段：
1. URP Shader 基础架构（Properties/Passes/CBUFFER/变体管理）— 第1-3天
2. Cel Shading 光照模型（smoothstep色阶化/URP光照API/GI/Emission/Occlusion）— 第4-7天
3. 描边系统（顶点挤出/相机适配/ZOffset/面部处理/阴影Bias）— 第8-11天
4. 整合与扩展（数据流回顾/Ramp贴图/性能优化/创意扩展）— 第12-14天

关键文件位置：
- UnityURPToonLitShaderExample/SimpleURPToonLitOutlineExample.shader（主Shader，5个Pass）
- UnityURPToonLitShaderExample/SimpleURPToonLitOutlineExample_LightingEquation.hlsl（光照公式）
- UnityURPToonLitShaderExample/SimpleURPToonLitOutlineExample_Shared.hlsl（数据结构与共享函数）
- UnityURPToonLitShaderExample/NiloOutlineUtil.hlsl（描边工具）
- UnityURPToonLitShaderExample/NiloZOffset.hlsl（深度偏移工具）
- UnityURPToonLitShaderExample/NiloInvLerpRemap.hlsl（插值工具）
- shader/ToonShader学习计划.md（完整学习计划文档）

当前进度：阶段一开始（2026-06-30）

用户前置知识：
- 基础光照模型（Lambert漫反射、Blinn-Phong高光）
- HLSL/GLSL基础语法
- Sea Shader学习中（已完成阶段一噪声基础、阶段二FBM、阶段三进行中）
- Unity基础使用经验

**Why:** 用户想系统学习Unity URP卡通渲染shader，从NiloCat的教学示例入手，逐步掌握URP shader开发。

**How to apply:** 当用户后续讨论Toon Shader学习进度时参考此计划。用户可能需要检查某个阶段的理解程度，或继续推进到下一阶段。

See also: [[sea-shader-study-plan]]
