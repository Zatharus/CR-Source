// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing3D.Light
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using System;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing3D
{
  public class Light
  {
    public Light()
    {
      this.DiffuseColor = (ColorF) Color.White;
      this.DiffusePower = 1f;
      this.SpecularPower = 0.0f;
      this.SpecularHardness = 0.3f;
      this.DistanceFallOff = false;
      this.Enabled = true;
    }

    public LightType LightType { get; set; }

    public Vector3 Position { get; set; }

    public Vector3 Direction { get; set; }

    public bool DistanceFallOff { get; set; }

    public ColorF DiffuseColor { get; set; }

    public float DiffusePower { get; set; }

    public ColorF SpecularColor { get; set; }

    public float SpecularPower { get; set; }

    public float SpecularHardness { get; set; }

    public bool Enabled { get; set; }

    public ColorF Calculate(Vector3 position, Vector3 viewDirection, Vector3 surfaceNormal)
    {
      switch (this.LightType)
      {
        case LightType.Point:
          return Light.CalclatePointLight(this, position, viewDirection, surfaceNormal).Diffuse;
        default:
          return Light.CalculateDirectionalLight(this, -this.Direction, 1f, viewDirection, surfaceNormal).Diffuse;
      }
    }

    public static LightingResult CalclatePointLight(
      Light light,
      Vector3 position,
      Vector3 viewDirection,
      Vector3 surfaceNormal)
    {
      Vector3 lightDirection = light.Position - position;
      float lightDistance = lightDirection.Length();
      return Light.CalculateDirectionalLight(light, lightDirection, lightDistance, viewDirection, surfaceNormal);
    }

    public static LightingResult CalculateDirectionalLight(
      Light light,
      Vector3 lightDirection,
      float lightDistance,
      Vector3 viewDirection,
      Vector3 surfaceNormal)
    {
      LightingResult directionalLight = new LightingResult();
      if ((double) light.DiffusePower <= 0.0)
        return directionalLight;
      lightDirection.Normalize();
      surfaceNormal.Normalize();
      if (light.DistanceFallOff)
        lightDistance *= lightDistance;
      else
        lightDistance = 1f;
      float num1 = Vector3.Dot(lightDirection, surfaceNormal).Clamp(0.0f, 1f);
      directionalLight.Diffuse = light.DiffuseColor * (num1 * light.DiffusePower / lightDistance);
      Vector3 a = 2f * Vector3.Dot(lightDirection, surfaceNormal) * surfaceNormal - lightDirection;
      a.Normalize();
      float num2 = (float) Math.Pow((double) Vector3.Dot(a, viewDirection).Clamp(0.0f, 1f), (double) light.SpecularHardness);
      directionalLight.Specular = light.SpecularColor * (num2 * light.SpecularPower / lightDistance);
      return directionalLight;
    }
  }
}
