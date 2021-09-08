using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wedestroy : MonoBehaviour
{
    public TypeEnum typeEnum = TypeEnum.Wall;

    private Progress progress;
    private BoxCollider[] _boxColliders;
    private Rigidbody[] _rigidbody;

    private ExplosiinParameters _explosiinParameters;//параметры д€ взрыва

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
        if(!_explosionWas)//если взрыв ещЄ не был
        {
            if (typeEnum == TypeEnum.Wall) GetComponent<BoxCollider>().enabled = false;//провер€ем если это этот объект можно разрушать

            if (explosionRadius) RadiusExplosion();

            foreach (var item in _boxColliders)//включаем коллайдер у елементов блока то есть его частей
            {
                item.enabled = true;
            }

            foreach (var item in _rigidbody)//делаем рабочей физику у частей блока
            {
                item.isKinematic = false;
            }

            VisualExplosion();

            _explosionWas = true;
        }

    }
    public void VisualExplosion()
    {
        ParticleSystem eff = Instantiate(_explosiinParameters.effectExplosion, transform.position, Quaternion.identity);//эекземпл€р эффекта

        eff.Play();//запуск

        cameraShake.InduceStress(1);//тр€ска камеры

        Handheld.Vibrate();//вибраци€

        _audio.Play();

        Destroy(eff, 1f);
    }
    public void RadiusExplosion()
    {
        Collider [] collider =  Physics.OverlapSphere(transform.position, _explosiinParameters.radius, _explosiinParameters.layerMask);//узнаем все кубы без их частей или элементов

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
            _clickWas = true;//регистрируем первый клик что бы до него очки не давали потому что мы не статичны и при запуске игры у нас уже при движении объектов начис€летс€ прогресс
        
        if(_clickWas)
        {
            switch (typeEnum)
            {
                case TypeEnum.Wall:
                    {
                        if (_explosionWas)//если этот объект уже
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
