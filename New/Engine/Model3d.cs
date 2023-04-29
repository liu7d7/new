using System.Globalization;
using New.Engine;
using OpenTK.Mathematics;

namespace New.Shared.Components;

public sealed class Model3d
{
  private static readonly Dictionary<string, Model3d> _components = new();
  private static readonly Shader _shader;
  private static readonly Ubo _ubo;

  public readonly Mesh<PC> Mesh;

  private readonly string _id;
  private readonly Face[] _faces;
  private readonly Vec<Matrix4> _models = new();
  private readonly Vector3[] _vertices;

  static Model3d()
  {
    _shader = new Shader("instanced", "john");
    _ubo = new Ubo(_shader, "_instanceInfo", "_model");
  }

  private Model3d(string path, Dictionary<string, uint> colors)
  {
    Color4 mat = new(1f, 1f, 1f, 1f);
    Vec<Face> faces = new();
    Vec<Vector3> vertices = new();
    Vector3 max = new(float.MinValue, float.MinValue, float.MinValue);
    Vector3 min = new(float.MaxValue, float.MaxValue, float.MaxValue);
    foreach (string line in File.ReadAllLines($"Resource/Model/{path}.obj"))
    {
      if (line.StartsWith("#")) continue;

      string[] parts = line.Split(' ');
      switch (parts[0])
      {
        case "v":
        {
          float x = parse_float(parts[1]), y = parse_float(parts[2]), z = parse_float(parts[3]);
          if (x > max.X) max.X = x;
          if (y > max.Y) max.Y = y;
          if (z > max.Z) max.Z = z;
          if (x < min.X) min.X = x;
          if (y < min.Y) min.Y = y;
          if (z < min.Z) min.Z = z;
          vertices.Add((x, y, z));
          break;
        }
        case "o":
          mat = Colors.NextColor();
          break;
        case "f":
        {
          string[] vt1 = parts[1].Split("/");
          string[] vt2 = parts[2].Split("/");
          string[] vt3 = parts[3].Split("/");
          Face face = new()
          {
            [0] = new VertexData(int.Parse(vt1[0]) - 1),
            [1] = new VertexData(int.Parse(vt2[0]) - 1),
            [2] = new VertexData(int.Parse(vt3[0]) - 1)
          };
          face[0].Color = face[1].Color = face[2].Color = (mat.R, mat.G, mat.B, mat.A);
          faces.Add(face);
          break;
        }
      }
    }

    _components[_id = path + colors.ContentToString()] = this;
    _vertices = vertices.ToArray();
    _faces = faces.ToArray();

    Mesh = new Mesh<PC>(DrawMode.TRIANGLE, _shader, false);
    to_mesh((0, 0, 0));
  }

  public void Scale(float scale)
  {
    Span<Vector3> vtx = _vertices;
    for (int i = 0; i < _vertices.Length; i++)
    {
      vtx[i].X *= scale;
      vtx[i].Y *= scale;
      vtx[i].Z *= scale;
    }

    to_mesh((0, 0, 0));
  }

  private float parse_float(string f)
  {
    return float.Parse(f, CultureInfo.InvariantCulture);
  }

  private void to_mesh(Vector3 pos)
  {
    Mesh.Begin();
    Span<Face> faces = _faces;
    for (int i = 0; i < _faces.Length; i++)
    {
      Vector3 vt1 = _vertices[faces[i][0].Pos];
      Vector3 vt2 = _vertices[faces[i][1].Pos];
      Vector3 vt3 = _vertices[faces[i][2].Pos];
      Mesh.Tri(Mesh.Put(new(vt1 + pos, faces[i][0].Color)), Mesh.Put(new(vt2 + pos, faces[i][1].Color)), Mesh.Put(new(vt3 + pos, faces[i][2].Color))
      );
    }

    Mesh.End();
  }

  public void Render(Vector3 pos)
  {
    _models.Add(Matrix4.CreateTranslation(pos) * RenderSystem.Model);
  }

  public void Render(float x, float y, float z)
  {
    _models.Add(Matrix4.CreateTranslation(x, y, z) * RenderSystem.Model);
  }

  public static void Draw()
  {
    foreach (KeyValuePair<string, Model3d> pair in _components)
    {
      Model3d model = pair.Value;
      if (model._models.Count == 0) continue;
      Mesh<PC> mesh = model.Mesh;

      Matrix4[] models = model._models.Items;

      int count = Math.Min(1024, model._models.Count);

      _shader.Bind();
      _ubo.put_all(ref models, count, 0);
      _ubo.bind_to(0);
      mesh.RenderInstanced(count);

      Shader.Unbind();
      model._models.Clear();
    }
  }

  public static Model3d Read(string path, Dictionary<string, uint> colors = null)
  {
    Dictionary<string, uint> def = colors ?? new Dictionary<string, uint>();
    return _components.ContainsKey(path + def.ContentToString())
      ? _components[path + def.ContentToString()]
      : new Model3d(path, def);
  }

  public static bool operator ==(Model3d a, Model3d b)
  {
    return a?._id == b?._id;
  }

  public static bool operator !=(Model3d a, Model3d b)
  {
    return !(a == b);
  }

  public override bool Equals(object obj)
  {
    if (obj is Model3d model)
      return model == this;
    return false;
  }

  public override int GetHashCode()
  {
    return _id.GetHashCode();
  }
}

public class VertexData
{
  public readonly int Pos;
  public Vector4 Color;

  public VertexData(int pos)
  {
    Pos = pos;
  }
}

public class Face
{
  private readonly VertexData[] _vertices;

  public Face()
  {
    _vertices = new VertexData[3];
  }

  public VertexData this[int idx]
  {
    get => _vertices[idx];
    init => _vertices[idx] = value;
  }
}