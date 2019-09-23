using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeCasterTutorial : MonoBehaviour {

    [SerializeField]
    private Es.InkPainter.Brush brush;

    [SerializeField]
    private MeshRenderer horn;

    private LineRenderer laserLine;
    public GameObject blasterView;

    public enum shotType {automatic, rayGun }
    public shotType shot = shotType.automatic;
    shotType lastShot;
    bool isRayGun = false;
    public AudioClip raySound, animalHitSound;
    AudioSource source;
    float fillPercent = 0;
    public UberMeter uberMeter;
    public float uberTime = 10f;
    bool canFillUber = true;
    public GameObject backgroundMusic_standard, backgroundMusic_uber;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        laserLine = GetComponentInChildren<LineRenderer>();
        lastShot = shot;
        UpdateShotType();
        source = GetComponent<AudioSource>();
    }
    void UpdateShotType()
    {
        source.Stop();
        lastShot = shot;
        isRayGun = false;
        source.loop = false;
        StopCoroutine(AutomaticShot());
        switch (shot)
        {
            case shotType.automatic:
                StartCoroutine(AutomaticShot());
                break;
            case shotType.rayGun:
                isRayGun = true;
                source.loop = true;
                source.Play();
                break;
        }
    }
    IEnumerator AutomaticShot()
    {
        while (true)
        {
            if (enabled)
            {
                Shoot();
            }
            yield return new WaitForSeconds(.5f);
        }
    }
    void Update()
    {
        if (lastShot != shot)
            UpdateShotType();
        if (isRayGun)
            Shoot();
    }
    void Shoot()
    {
        laserLine.SetPosition(0, horn.transform.position);
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            laserLine.SetPosition(1, hit.point);
            if (hit.collider.CompareTag("Animal"))
            {
                source.PlayOneShot(animalHitSound);
                hit.collider.tag = "Environment";
                Material mat = hit.collider.GetComponent<Renderer>().material;
                print(hit.collider.gameObject.name + " : " + mat.ToString());
                if (mat.color != null && mat.color != brush.Color)
                    StartCoroutine(ChangeAnimalColor(mat, mat.color, brush.Color));
                ParticleSystem ps = hit.collider.GetComponentInParent<ParticleSystem>();
                if (ps != null)
                    ps.Play();
                Animator anim = hit.collider.GetComponentInParent<Animator>();
                if (anim != null && !anim.enabled)
                {
                    anim.speed = 0;
                    anim.enabled = true;
                    // hit.collider.GetComponentInParent<Wander>().enabled = true;
                    StartCoroutine(ActivateAnimalAnimator(anim));
                }
                if (canFillUber)
                {
                    if (uberMeter.AddUberPoint())
                    {
                        StartCoroutine(ActivateUberRayGun());
                    }
                }
            }
            else if (hit.collider.CompareTag("Environment"))
            {
                Material mat = hit.collider.GetComponent<Renderer>().material;
                if (mat.color != null && mat.color != brush.Color)
                    StartCoroutine(ChangeAnimalColor(mat, mat.color, brush.Color));
            }
            else
            {
                UpdatePaintColor(hit);
                UpdatePaint(hit);
            }
            if (shot != shotType.rayGun)
                StartCoroutine(ShotLaser());
        }
        else
        {
            laserLine.SetPosition(1, transform.position + transform.forward * 100);
            StartCoroutine(ShotLaser());
        }
    }
    IEnumerator ActivateUberRayGun()
    {
        blasterView.SetActive(true);
        backgroundMusic_standard.SetActive(false);
        backgroundMusic_uber.SetActive(true);
        canFillUber = false;
        shot = shotType.rayGun;
        yield return new WaitForSeconds(20f);
        shot = shotType.automatic;
        canFillUber = true;
        backgroundMusic_uber.SetActive(false);
        backgroundMusic_standard.SetActive(true);
        blasterView.SetActive(false);
    }
    IEnumerator ShotLaser()
    {
        yield return new WaitForSeconds(.1f);
        laserLine.SetPosition(1, transform.position);
    }
    IEnumerator ActivateAnimalAnimator(Animator animator)
    {
        for (float i = 0f; i <= 1f; i += 0.0025f)
        {
            animator.speed = i;
            yield return null;
        }
        yield break;
    }

    IEnumerator ChangeAnimalColor(Material material, Color oldColor, Color newColor)
    {
        for (float i = 0f; i <= 1f; i += 0.01f)
        {
            material.color = Color.Lerp(oldColor, newColor, i);
            yield return null;
        }
        yield break;
    }

    private bool UpdatePaintColor(RaycastHit hit)
    {
        PaintCan paintCan = hit.transform.GetComponent<PaintCan>();
        if (paintCan != null)
        {
            brush.Color = paintCan.color;
            horn.material.color = paintCan.color;
            return true;
        }
        return false;
    }

    private bool UpdatePaint(RaycastHit hit)
    {
        Es.InkPainter.InkCanvas paintObject = hit.transform.GetComponent<Es.InkPainter.InkCanvas>();
        if (paintObject != null)
        {
            //paintObject.PaintUVDirect(brush, hit.textureCoord);
            paintObject.Paint(brush, hit);
            return true;
        }
        return false;
    }
}
