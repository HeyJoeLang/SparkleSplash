using System.Collections;
using UnityEngine;

public class GazeCaster : GazeModeListener {

    #region variables
    
    public PlayerScoreEvent playerScoreEvent;
    public Movement playerMove;
    public float environment_sfx_PitchMin = 1f;
    public float environment_sfx_PitchMax = 1f;

    [SerializeField]    private LineRenderer line_preview;
    [SerializeField]    private Material line_previewMatNormal;
    [SerializeField]    private Material line_previewMatMagic;
    [SerializeField]    private MeshRenderer render_horn;
    [SerializeField]    private Canvas ui_reticle;
    [SerializeField]    private AudioClip sfx_Environment;
    [SerializeField]    private Transform trans_colorSelection;
    [SerializeField]    private Es.InkPainter.Brush brush;
    [SerializeField]    private ParticleSystem particleSystem_hit;
    [SerializeField]    private Material environmentMat;

    private Color color_brush;
    private AudioSource source;
    private bool lastFrameWasUI = false;
    #endregion
    #region Unity Callbacks

    private void Start()
    {
        line_previewMatNormal = line_preview.material;
        playerScoreEvent.score = 0;
        ui_reticle.gameObject.SetActive(false);
        Vector3 originalScale = ui_reticle.transform.localScale;
        ui_reticle.transform.localScale = originalScale * 5;
        source = GetComponent<AudioSource>();
        brush.Color = Color.blue;

        StartCoroutine("TimedBurst");
    }


    void FixedUpdate()
    {
        switch (gazeModeEvent.gaze_state)
        {
            case GazeModeEvent.GAZE_MODE.MAGIC_BLASTER:
                Blast();
                break;
            case GazeModeEvent.GAZE_MODE.TIMED_SHOT:
                PreviewLine();
                break;
            default:
                break;
        }
    }
    public override void GazeToMagicBlaster()
    {
        base.GazeToMagicBlaster();
        line_preview.material = line_previewMatMagic;
        playerMove.speed = 10;
        StopAllCoroutines();
        source.Stop();
    }
    public override void GazeToTimedShot()
    {
        base.GazeToTimedShot();
        line_preview.material = line_previewMatNormal;
        playerMove.speed = 5;
        StopAllCoroutines();
        StartCoroutine("TimedBurst");
    }

    #endregion
    #region Casting Gazes
    void PreviewLine()
    {
        CastGaze(line_preview);
    }
    void CastGaze(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, render_horn.transform.position);
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(lastFrameWasUI == true)
            ui_reticle.gameObject.SetActive(false);
        if (Physics.Raycast(ray, out hit, 200))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                ui_reticle.transform.position = hit.point - (hit.point - transform.position) * .1f;
                ui_reticle.gameObject.SetActive(true);
                PaintCan can = hit.collider.GetComponent<PaintCan>();
                if (can != null)
                {
                   UpdatePaintColor(hit);
                   trans_colorSelection.position = can.transform.position; 
                }
                lineRenderer.SetPosition(1, render_horn.transform.position);
                StopCoroutine("TimedBurst");
                lastFrameWasUI = true;
            }
            else
            {
                if(lastFrameWasUI)
                {
                    lastFrameWasUI = false;
                    switch(gazeModeEvent.gaze_state)
                    {
                        case GazeModeEvent.GAZE_MODE.TIMED_SHOT:
                            StartCoroutine("TimedBurst");
                            break;
                        case GazeModeEvent.GAZE_MODE.MAGIC_BLASTER:
                            lineRenderer.gameObject.SetActive(true);
                            break;
                    }
                }
                ui_reticle.gameObject.SetActive(false);
                lineRenderer.SetPosition(1, hit.point);
            }
        }
        else
        {
            lineRenderer.SetPosition(1, render_horn.transform.position + transform.forward * 100);
        }
    }
    void Blast()
    {
        CastGaze(line_preview);
        line_preview.material.SetFloat("_Blend", 1f);
        Ray ray = new Ray(render_horn.transform.position + transform.forward, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3000))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("UI"))
            {
                particleSystem_hit.transform.position = hit.point;
                particleSystem_hit.Play();
            }
            if (!hit.collider.CompareTag("Animal") && !hit.collider.CompareTag("Environment") && gazeModeEvent.gaze_state == GazeModeEvent.GAZE_MODE.TIMED_SHOT)
            {
                source.pitch = Random.Range(environment_sfx_PitchMin, environment_sfx_PitchMax);
                source.PlayOneShot(sfx_Environment);
            }
            if (hit.collider.CompareTag("Animal"))
            {
                playerScoreEvent.Raise(100);
                StartCoroutine(ChangeMaterialColor(hit.collider.GetComponent<Renderer>().material));
                hit.collider.tag = "Animal_Hit";

                ParticleSystem ps = hit.collider.GetComponentInParent<ParticleSystem>();
                if (ps != null)
                    ps.Play();
                Animator anim = hit.collider.GetComponentInParent<Animator>();
                if (anim != null && !anim.enabled)
                {
                    anim.speed = 2;
                    anim.enabled = true;
                }
            }
        }
        if(gazeModeEvent.gaze_state == GazeModeEvent.GAZE_MODE.TIMED_SHOT)
            StartCoroutine("VisualizeBlast");
    }
    #endregion
    #region Supporting Coroutines

    IEnumerator TimedBurst()
    {
        while (true)
        {
            if (enabled)
                Blast();
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator VisualizeBlast()
    {
        for (float i = 1f; i >= 0f; i -= Time.deltaTime * 2)
        {
            line_preview.material.SetFloat("_Blend", i);
            yield return null;
        }
    }

    IEnumerator ChangeMaterialColor(Material material)
    {
        StartCoroutine("FinalizeChangeMaterialColor", material);
        if (!material.HasProperty("_Threshold"))
            yield break;
        if (material.GetFloat("_Threshold") != 1.1f)
                yield break;
        for (float i = 1.1f; i >= 0f; i -= Time.deltaTime * 2)
        {
            material.SetFloat("_Threshold", i);
            yield return null;
        }
        material.SetFloat("_Threshold", 0);
        yield break;
    }
    IEnumerator FinalizeChangeMaterialColor(Material material)
    {
        yield return new WaitForSeconds(1);
        material.SetFloat("_Threshold", 0);
    }

    #endregion
    #region Modify Colors

    void UpdatePaintColor(RaycastHit hit)
    {
        PaintCan paintCan = hit.transform.GetComponent<PaintCan>();
        if (paintCan != null)
        {
            color_brush = paintCan.color;
            brush.Color = render_horn.material.color = new Color(paintCan.color.r, paintCan.color.g, paintCan.color.b);
        }
    }

    bool UpdatePaint(RaycastHit hit)
    {
        Es.InkPainter.InkCanvas paintObject = hit.transform.GetComponent<Es.InkPainter.InkCanvas>();
        if (paintObject != null)
        {
            paintObject.Paint(brush, hit);
            return true; 
        }
        return false; 
    }

    #endregion
}

