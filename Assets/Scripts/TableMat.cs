using UnityEngine;

public class TableMat : MonoBehaviour
{
    private SpriteRenderer _spriteRenderes;
    private Material _materials;

    [SerializeField] private float _hologramSpeed = 0.05f;
    private float hologram_position;

    private int _verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    void Start()
    {
        _spriteRenderes = GetComponent<SpriteRenderer>();
        _materials = _spriteRenderes.material;

        hologram_position = 0f;
    }

    
    void Update()
    {
        hologram_position += _hologramSpeed * Time.deltaTime;

        _materials.SetFloat(_verticalDissolveAmount, hologram_position);

        if(hologram_position > 0.43f)
        {
            hologram_position = 0;
        }
    }
}
