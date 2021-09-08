using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wedestroy : MonoBehaviour
{
    public TypeEnum typeEnum = TypeEnum.Wall;

    private Progress progress;
    private BoxCollider[] _boxColliders;
    private Rigidbody[] _rigidbody;

    private ExplosiinParameters _explosiinParameters;//��������� �� ������

    private StressReceiver cameraShake;

    private AudioSource _audio;

    private bool _explosionWas;

    private bool _clickWas;

    void Start()
    {
        _boxColliders = GetComponentsInChildren<BoxCollider>();
        _rigidbody = GetComponentsInChildren<Rigidbody>();
        progress = FindObjectOfType<Progress>();
        _audio = progress.GetComponent<AudioSource>();

        _explosiinParameters = Resources.Load<ExplosiinParameters>("OptionExplosion/CubeExpl");
        cameraShake = FindObjectOfType<StressReceiver>();

        _startPos = Mathf.Abs(transform.position.y);
    }
    private void OnMouseDown()
    {
        Explosion(true);
    }
    public void Explosion(bool explosionRadius)
    {
        if(!_explosionWas)//���� ����� ��� �� ���
        {
            if (typeEnum == TypeEnum.Wall) GetComponent<BoxCollider>().enabled = false;//��������� ���� ��� ���� ������ ����� ���������

            if (explosionRadius) RadiusExplosion();

            foreach (var item in _boxColliders)//�������� ��������� � ��������� ����� �� ���� ��� ������
            {
                item.enabled = true;
            }

            foreach (var item in _rigidbody)//������ ������� ������ � ������ �����
            {
                item.isKinematic = false;
            }

            VisualExplosion();

            _explosionWas = true;
        }

    }
    public void VisualExplosion()
    {
        ParticleSystem eff = Instantiate(_explosiinParameters.effectExplosion, transform.position, Quaternion.identity);//���������� �������

        eff.Play();//������

        cameraShake.InduceStress(1);//������ ������

        Handheld.Vibrate();//��������

        _audio.Play();

        Destroy(eff, 1f);
    }
    public void RadiusExplosion()
    {
        Collider [] collider =  Physics.OverlapSphere(transform.position, _explosiinParameters.radius, _explosiinParameters.layerMask);//������ ��� ���� ��� �� ������ ��� ���������

        foreach(var item in collider)
        {
           item.GetComponent<Wedestroy>().Explosion(false);
        }
    }
    private float _startPos;

    private float getPos;

    void FixedUpdate()
    {
       if (Input.GetMouseButtonDown(0) && !_clickWas)
            _clickWas = true;//������������ ������ ���� ��� �� �� ���� ���� �� ������ ������ ��� �� �� �������� � ��� ������� ���� � ��� ��� ��� �������� �������� ����������� ��������
        
        if(_clickWas)
        {
            switch (typeEnum)
            {
                case TypeEnum.Wall:
                    {
                        if (_explosionWas)//���� ���� ������ ���
                        {
                            foreach (var item in _rigidbody)
                            {
                                progress.Write(Mathf.Abs(item.transform.localPosition.y) * item.velocity.magnitude * GetComponent<Rigidbody>().mass * 0.000026f);
                            }
                        }
                        break;
                    }
                case TypeEnum.Celling:
                    {
                            progress.Write(Mathf.Abs(transform.localPosition.y) * GetComponent<Rigidbody>().velocity.magnitude * GetComponent<Rigidbody>().mass * 0.0023f);
                        break;
                    }
            }
        }
        

    }
}
