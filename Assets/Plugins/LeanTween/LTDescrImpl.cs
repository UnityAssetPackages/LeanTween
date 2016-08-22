using UnityEngine;
using System;
using System.Collections.Generic;

/**
* Internal Representation of a Tween<br>
* <br>
* This class represents all of the optional parameters you can pass to a method (it also represents the internal representation of the tween).<br><br>
* <strong id='optional'>Optional Parameters</strong> are passed at the end of every method:<br> 
* <br>
* &nbsp;&nbsp;<i>Example:</i><br>
* &nbsp;&nbsp;LeanTween.moveX( gameObject, 1f, 1f).setEase( <a href="LeanTweenType.html">LeanTweenType</a>.easeInQuad ).setDelay(1f);<br>
* <br>
* You can pass the optional parameters in any order, and chain on as many as you wish.<br>
* You can also <strong>pass parameters at a later time</strong> by saving a reference to what is returned.<br>
* <br>
* Retrieve a <strong>unique id</strong> for the tween by using the "id" property. You can pass this to LeanTween.pause, LeanTween.resume, LeanTween.cancel, LeanTween.isTweening methods<br>
* <br>
* &nbsp;&nbsp;<h4>Example:</h4>
* &nbsp;&nbsp;int id = LeanTween.moveX(gameObject, 1f, 3f).id;<br>
* <div style="color:gray">&nbsp;&nbsp;// pause a specific tween</div>
* &nbsp;&nbsp;LeanTween.pause(id);<br>
* <div style="color:gray">&nbsp;&nbsp;// resume later</div>
* &nbsp;&nbsp;LeanTween.resume(id);<br>
* <div style="color:gray">&nbsp;&nbsp;// check if it is tweening before kicking of a new tween</div>
* &nbsp;&nbsp;if( LeanTween.isTweening( id ) ){<br>
* &nbsp;&nbsp; &nbsp;&nbsp;	LeanTween.cancel( id );<br>
* &nbsp;&nbsp; &nbsp;&nbsp;	LeanTween.moveZ(gameObject, 10f, 3f);<br>
* &nbsp;&nbsp;}<br>
* @class LTDescr
* @constructor
*/

public class LTDescrImpl : LTDescr {
	public bool toggle { get; set; }
	public bool useEstimatedTime { get; set; }
	public bool useFrames { get; set; }
	public bool useManualTime { get; set; }
	public bool hasInitiliazed { get; set; }
	public bool hasPhysics { get; set; }
	public bool onCompleteOnRepeat { get; set; }
	public bool onCompleteOnStart { get; set; }
	public bool useRecursion { get; set; }
    public float ratioPassed { get; set; }
	public float passed { get; set; }
	public float delay { get; set; }
	public float time { get; set; }
	public float speed{get; set;}
	public float lastVal { get; set; }
	private uint _id;
	public int loopCount { get; set; }
	public uint counter { get; set; }
	public float direction { get; set; }
	public float directionLast { get; set; }
	public float overshoot { get; set; }
	public float period { get; set; }
	public bool destroyOnComplete { get; set; }
	public Transform trans { get; set; }
	public Transform toTrans { get; set; }
	public LTRect ltRect { get; set; }
	internal Vector3 fromInternal;
	public Vector3 from { get { return this.fromInternal; } set { this.fromInternal = value; } }
	internal Vector3 toInternal;
	public Vector3 to { get { return this.toInternal; } set { this.toInternal = value; } }
	internal Vector3 diff;
	internal Vector3 diffDiv2;
	public TweenAction type { get; set; }
	public LeanTweenType tweenType { get; set; }
	public LeanTweenType loopType { get; set; }

	public bool hasUpdateCallback { get; set; }


	public EaseTypeDelegate easeMethod { get; set; }
	public ActionMethodDelegate easeInternal {get; set; }
	public delegate Vector3 EaseTypeDelegate();
	public delegate void ActionMethodDelegate();
	#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
	public SpriteRenderer spriteRen { get; set; }
	#endif

	#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
	public RectTransform rectTransform;
    public UnityEngine.UI.Text uiText;
    public UnityEngine.UI.Image uiImage;
    public UnityEngine.Sprite[] sprites;
	#endif

	public LTDescrOptional _optional = new LTDescrOptional();

	private static uint global_counter = 0;

    public override string ToString(){
		return (trans!=null ? "gameObject:"+trans.gameObject : "gameObject:null")+" toggle:"+toggle+" passed:"+passed+" time:"+time+" delay:"+delay+" direction:"+direction+" from:"+from+" to:"+to+" diff:"+diff+" type:"+type+" ease:"+tweenType+" useEstimatedTime:"+useEstimatedTime+" id:"+id+" hasInitiliazed:"+hasInitiliazed;
	}

	public LTDescrImpl(){

	}

	[System.Obsolete("Use 'LeanTween.cancel( id )' instead")]
	public LTDescr cancel( GameObject gameObject ){
		// Debug.Log("canceling id:"+this._id+" this.uniqueId:"+this.uniqueId+" go:"+this.trans.gameObject);
		if(gameObject==this.trans.gameObject)
			LeanTween.removeTween((int)this._id, this.uniqueId);
		return this;
	}

	public int uniqueId{
		get{ 
			uint toId = _id | counter << 16;

			/*uint backId = toId & 0xFFFF;
			uint backCounter = toId >> 16;
			if(_id!=backId || backCounter!=counter){
				Debug.LogError("BAD CONVERSION toId:"+_id);
			}*/

			return (int)toId;
		}
	}

	public int id{
		get{ 
			return uniqueId;
		}
	}

	public LTDescrOptional optional{
		get{ 
			return _optional;
		}
		set{
			this._optional = optional;
		}
	}

	public void reset(){
		this.toggle = this.useRecursion = true;
        this.trans = null;
		this.passed = this.delay = this.lastVal = 0.0f;
		this.hasUpdateCallback = this.useEstimatedTime = this.useFrames = this.hasInitiliazed = this.onCompleteOnRepeat = this.destroyOnComplete = this.onCompleteOnStart = this.useManualTime = false;
		this.tweenType = LeanTweenType.linear;
		this.loopType = LeanTweenType.once;
		this.loopCount = 0;
		this.direction = this.directionLast = this.overshoot = 1.0f;
		this.period = 0.3f;
		this.speed = -1f;
		this._optional.reset();
		
		global_counter++;
		if(global_counter>0x8000)
			global_counter = 0;
	}


	// This method is only for internal use
	public void init(){
		this.hasInitiliazed = true;

        if (this.time <= 0f) { // avoid dividing by zero
            this.ratioPassed = 1f;
        }
//		this._optional = new LTDescrOptional();

		if (this._optional.onStart != null){
			this._optional.onStart();
        }		 

		// Initialize From Values
		switch(this.type){
			case TweenAction.MOVE:
				this.from = trans.position;
				this.easeInternal = moveInternal;
				break;
			case TweenAction.MOVE_TO_TRANSFORM:
				this.from = trans.position; break;
			case TweenAction.MOVE_X:
				this.fromInternal.x = trans.position.x; break;
			case TweenAction.MOVE_Y:
				this.fromInternal.x = trans.position.y; break;
			case TweenAction.MOVE_Z:
				this.fromInternal.x = trans.position.z; break;
			case TweenAction.MOVE_LOCAL_X:
				this.fromInternal.x = trans.localPosition.x; break;
			case TweenAction.MOVE_LOCAL_Y:
				this.fromInternal.x = trans.localPosition.y; break;
			case TweenAction.MOVE_LOCAL_Z:
				this.fromInternal.x = trans.localPosition.z; break;
			case TweenAction.SCALE_X:
				this.fromInternal.x = trans.localScale.x; break;
			case TweenAction.SCALE_Y:
				this.fromInternal.x = trans.localScale.y; break;
			case TweenAction.SCALE_Z:
				this.fromInternal.x = trans.localScale.z; break;
			case TweenAction.ALPHA:
				#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
					if(trans.gameObject.renderer){
						this.fromInternal.x = trans.gameObject.renderer.material.color.a;
					}else if(trans.childCount>0){
						foreach (Transform child in trans) {
							if(child.gameObject.renderer!=null){
								Color col = child.gameObject.renderer.material.color;
								this.fromInternal.x = col.a;
								break;
	    					}
						}
					}
					break;	
				#else
					SpriteRenderer ren = trans.gameObject.GetComponent<SpriteRenderer>();
					if(ren!=null){
						this.fromInternal.x = ren.color.a;
					}else{
						if(trans.gameObject.GetComponent<Renderer>()!=null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_Color")){
							this.fromInternal.x = trans.gameObject.GetComponent<Renderer>().material.color.a;
						}else if(trans.gameObject.GetComponent<Renderer>()!=null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_TintColor")){
							Color col = trans.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
							this.fromInternal.x = col.a;
						}else if(trans.childCount>0){
							foreach (Transform child in trans) {
								if(child.gameObject.GetComponent<Renderer>()!=null){
									Color col = child.gameObject.GetComponent<Renderer>().material.color;
									this.fromInternal.x = col.a;
									break;
		    					}
							}
						}
					}
					break;
				#endif
			case TweenAction.MOVE_LOCAL:
				this.from = trans.localPosition; break;
			case TweenAction.MOVE_CURVED:
			case TweenAction.MOVE_CURVED_LOCAL:
			case TweenAction.MOVE_SPLINE:
			case TweenAction.MOVE_SPLINE_LOCAL:
				this.fromInternal.x = 0; break;
			case TweenAction.ROTATE:
				this.from = trans.eulerAngles; 
				this.to = new Vector3(LeanTween.closestRot( this.fromInternal.x, this.toInternal.x), LeanTween.closestRot( this.from.y, this.to.y), LeanTween.closestRot( this.from.z, this.to.z));
				break;
			case TweenAction.ROTATE_X:
				this.fromInternal.x = trans.eulerAngles.x; 
				this.toInternal.x = LeanTween.closestRot( this.fromInternal.x, this.toInternal.x);
				break;
			case TweenAction.ROTATE_Y:
				this.fromInternal.x = trans.eulerAngles.y; 
				this.toInternal.x = LeanTween.closestRot( this.fromInternal.x, this.toInternal.x);
				break;
			case TweenAction.ROTATE_Z:
				this.fromInternal.x = trans.eulerAngles.z; 
				this.toInternal.x = LeanTween.closestRot( this.fromInternal.x, this.toInternal.x);
				break;
			case TweenAction.ROTATE_AROUND:
				this.lastVal = 0.0f; // ["last"]
				this.fromInternal.x = 0.0f;
				this._optional.origRotation = trans.rotation; // optional["origRotation"
				break;
			case TweenAction.ROTATE_AROUND_LOCAL:
				this.lastVal = 0.0f; // optional["last"]
				this.fromInternal.x = 0.0f;
				this._optional.origRotation = trans.localRotation; // optional["origRotation"
				break;
			case TweenAction.ROTATE_LOCAL:
				this.from = trans.localEulerAngles; 
				this.to = new Vector3(LeanTween.closestRot( this.fromInternal.x, this.toInternal.x), LeanTween.closestRot( this.from.y, this.to.y), LeanTween.closestRot( this.from.z, this.to.z));
				break;
			case TweenAction.SCALE:
				this.from = trans.localScale; break;
			case TweenAction.GUI_MOVE:
				this.from = new Vector3(this._optional.ltRect.rect.x, this._optional.ltRect.rect.y, 0); break;
			case TweenAction.GUI_MOVE_MARGIN:
				this.from = new Vector2(this._optional.ltRect.margin.x, this._optional.ltRect.margin.y); break;
			case TweenAction.GUI_SCALE:
				this.from = new Vector3(this._optional.ltRect.rect.width, this._optional.ltRect.rect.height, 0); break;
			case TweenAction.GUI_ALPHA:
				this.fromInternal.x = this._optional.ltRect.alpha; break;
			case TweenAction.GUI_ROTATE:
				if(this._optional.ltRect.rotateEnabled==false){
					this._optional.ltRect.rotateEnabled = true;
					this._optional.ltRect.resetForRotation();
				}
				
				this.fromInternal.x = this._optional.ltRect.rotation; break;
			case TweenAction.ALPHA_VERTEX:
				this.fromInternal.x = trans.GetComponent<MeshFilter>().mesh.colors32[0].a;
				break;
			case TweenAction.CALLBACK:
				break;
			case TweenAction.CALLBACK_COLOR:
				this.diff = new Vector3(1.0f,0.0f,0.0f);
				break;
			case TweenAction.COLOR:
				#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
					if(trans.gameObject.renderer){
						Color col = trans.gameObject.renderer.material.color;
						this.setFromColor( col );
					}else if(trans.childCount>0){
						foreach (Transform child in trans) {
							if(child.gameObject.renderer!=null){
								Color col = child.gameObject.renderer.material.color;
								this.setFromColor( col );
								break;
	    					}
						}
					}
				#else
					SpriteRenderer renColor = trans.gameObject.GetComponent<SpriteRenderer>();
                    if(renColor!=null){
                        Color col = renColor.color;
                        this.setFromColor( col );
                    }else{
                        if(trans.gameObject.GetComponent<Renderer>()!=null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_Color")){
							Color col = trans.gameObject.GetComponent<Renderer>().material.color;
							this.setFromColor( col );
						}else if(trans.gameObject.GetComponent<Renderer>()!=null && trans.gameObject.GetComponent<Renderer>().material.HasProperty("_TintColor")){
							Color col = trans.gameObject.GetComponent<Renderer>().material.GetColor ("_TintColor");
							this.setFromColor( col );
						}else if(trans.childCount>0){
							foreach (Transform child in trans) {
								if(child.gameObject.GetComponent<Renderer>()!=null){
									Color col = child.gameObject.GetComponent<Renderer>().material.color;
									this.setFromColor( col );
									break;
		    					}
							}
						}
                    }
				#endif
				break;
			#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
			case TweenAction.CANVAS_ALPHA:
				this.uiImage = trans.gameObject.GetComponent<UnityEngine.UI.Image>();
                if(this.uiImage != null){
	                this.fromInternal.x = this.uiImage.color.a;
                }else{
                	this.fromInternal.x = 1f;
                }
                break;
            case TweenAction.CANVAS_COLOR:
                this.uiImage = trans.gameObject.GetComponent<UnityEngine.UI.Image>();
                if(this.uiImage != null){
	               this.setFromColor( this.uiImage.color );
	            }else{
                	this.setFromColor( Color.white );
				}
                break;
            case TweenAction.CANVASGROUP_ALPHA:
				this.fromInternal.x = trans.gameObject.GetComponent<CanvasGroup>().alpha;
                break;
            case TweenAction.TEXT_ALPHA:
                this.uiText = trans.gameObject.GetComponent<UnityEngine.UI.Text>();
                if (this.uiText != null){
                    this.fromInternal.x = this.uiText.color.a;
                }else{
                	this.fromInternal.x = 1f;
                }
                break;
            case TweenAction.TEXT_COLOR:
                this.uiText = trans.gameObject.GetComponent<UnityEngine.UI.Text>();
                if (this.uiText != null){
                    this.setFromColor( this.uiText.color );
                }else{
                	this.setFromColor( Color.white );
                }
                break;
			case TweenAction.CANVAS_MOVE:
				this.fromInternal = this.rectTransform.anchoredPosition3D; break;
			case TweenAction.CANVAS_MOVE_X:
				this.fromInternal.x = this.rectTransform.anchoredPosition3D.x; break;
			case TweenAction.CANVAS_MOVE_Y:
				this.fromInternal.x = this.rectTransform.anchoredPosition3D.y; break;
			case TweenAction.CANVAS_MOVE_Z:
				this.fromInternal.x = this.rectTransform.anchoredPosition3D.z; break;
			case TweenAction.CANVAS_ROTATEAROUND:
			case TweenAction.CANVAS_ROTATEAROUND_LOCAL:
				this.lastVal = 0.0f;
				this.fromInternal.x = 0.0f;
				this._optional.origRotation = this.rectTransform.rotation;
				break;
			case TweenAction.CANVAS_SCALE:
				this.from = this.rectTransform.localScale; break;
			case TweenAction.CANVAS_PLAYSPRITE:
				this.uiImage = trans.gameObject.GetComponent<UnityEngine.UI.Image>();
				this.fromInternal.x = 0f;
				break;
			#endif
		}

		this.diff = this.to - this.from;
		this.diffDiv2 = this.diff * 0.5f;
		if(this.onCompleteOnStart){
			if(this._optional.onComplete!=null){
				this._optional.onComplete();
			}else if(this._optional.onCompleteObject!=null){
				this._optional.onCompleteObject(this._optional.onCompleteParam);
			}
		}

		if(this.speed>=0){
			if(this.type==TweenAction.MOVE_CURVED || this.type==TweenAction.MOVE_CURVED_LOCAL){
				this.time = this._optional.path.distance / this.speed ;
			}else if(this.type==TweenAction.MOVE_SPLINE || this.type==TweenAction.MOVE_SPLINE_LOCAL){
				this.time = this._optional.spline.distance/ this.speed;
			}else{
				this.time = (this.to - this.from).magnitude / this.speed;
			}
		}
	}

	public LTDescr setFromColor( Color col ){
		this.from = new Vector3(0.0f, col.a, 0.0f);
		this.diff = new Vector3(1.0f,0.0f,0.0f);
		this._optional.axis = new Vector3( col.r, col.g, col.b );
		return this;
	}
        
//    private static float ratioPassed;
    //private static float to = 1.0f;
	public static float val;
	public static Vector3 newVect;

    public static float dt;
    private static bool isTweenFinished;

	private static bool hasExtraOnCompletes = false;
	private static bool usesNormalDt = true;

	private void moveInternal(){
		trans.position = easeMethod();
//		Debug.Log("trans.position x:"+trans.position.x+" y:"+trans.position.y+" z:"+trans.position.z);
//		Debug.Log("diffDiv2 x:"+this.diffDiv2.x+" y:"+this.diffDiv2.y+" z:"+this.diffDiv2.z);
	}

	public bool update2(){
		isTweenFinished = false;

		if(usesNormalDt){
			dt = LeanTween.dtActual;
		}
//		else if( this.useEstimatedTime ){
//			dt = LeanTween.dtEstimated;
//		}else if( this.useFrames ){
//			dt = 1;
//		}else if( this.useManualTime ){
//			dt = LeanTween.dtManual;
//		}else if(this.direction==0f){
//			dt = 0f;
//		}
		// check to see if delay has shrunk enough
//		if(dt!=0f){
			if(this.delay<=0f){
				if(trans==null)
					return true;	

				// initialize if has not done so yet
				if(!this.hasInitiliazed)
					this.init();

				this.passed += dt*this.direction;

				if(this.direction>0f){
					isTweenFinished = this.passed>=this.time;
				}else{
					isTweenFinished = this.passed<=0f;
				}
				this.ratioPassed = isTweenFinished ? Mathf.Clamp01(this.passed / this.time) : this.passed / this.time; // need to clamp when finished so it will finish at the exact spot and not overshoot

				this.easeInternal();

				if(this.hasUpdateCallback){
//					if(this._optional.onUpdateFloat!=null){
//						this._optional.onUpdateFloat(val);
//					}
//					if (this._optional.onUpdateFloatRatio != null){
//						this._optional.onUpdateFloatRatio(val,ratioPassed);
//					}else if(this._optional.onUpdateFloatObject!=null){
//						this._optional.onUpdateFloatObject(val, this._optional.onUpdateParam);
//					}else if(this._optional.onUpdateVector3Object!=null){
//						this._optional.onUpdateVector3Object(newVect, this._optional.onUpdateParam);
//					}else if(this._optional.onUpdateVector3!=null){
//						this._optional.onUpdateVector3(newVect);
//					}else if(this._optional.onUpdateVector2!=null){
//						this._optional.onUpdateVector2(new Vector2(newVect.x,newVect.y));
//					}
				}
			}else{
				this.delay -= dt;
				// Debug.Log("dt:"+dt+" tween:"+i+" tween:"+tween);
	//			if(this.delay<0f){
	//				this.passed = 0.0f;//-tween.delay
	//				this.delay = 0.0f;
	//			}
			}
//		}

//		Debug.Log("tween:"+this+" dt:"+dt);

		// increment or flip tween

		if(isTweenFinished){
			this.loopCount--;
			this.direction = 0f - this.direction;
//			Debug.Log("this.loopCount:" + this.loopCount);

//				if((this.loopCount<=0 && this.type==TweenAction.CALLBACK) || this.onCompleteOnRepeat){
					if(hasExtraOnCompletes){ // ToDo: Only turn on for extra on completes
						if(this.type==TweenAction.GUI_ROTATE)
							this._optional.ltRect.rotateFinished = true;
						if(this.type==TweenAction.DELAYED_SOUND){
							AudioSource.PlayClipAtPoint((AudioClip)this._optional.onCompleteParam, this.to, this.from.x);
						}
						if(this._optional.onComplete!=null){
							this._optional.onComplete();
						}else if(this._optional.onCompleteObject!=null){
							this._optional.onCompleteObject(this._optional.onCompleteParam);
						}
					}
//				}
			isTweenFinished = this.loopCount == 0 || this.loopType == LeanTweenType.once; // only return true if it is fully complete
		}

		return isTweenFinished;
	}

    private static void alphaRecursive( Transform transform, float val, bool useRecursion = true){
        Renderer renderer = transform.gameObject.GetComponent<Renderer>();
        if(renderer!=null){
            foreach(Material mat in renderer.materials){
                if(mat.HasProperty("_Color")){
                    mat.color = new Color( mat.color.r, mat.color.g, mat.color.b, val);
                }else if(mat.HasProperty("_TintColor")){
                    Color col = mat.GetColor ("_TintColor");
                    mat.SetColor("_TintColor", new Color( col.r, col.g, col.b, val));
                }
            }
        }
        if(useRecursion && transform.childCount>0){
            foreach (Transform child in transform) {
                alphaRecursive(child, val);
            }
        }
    }

    private static void colorRecursive( Transform transform, Color toColor, bool useRecursion = true ){
        Renderer ren = transform.gameObject.GetComponent<Renderer>();
        if(ren!=null){
            foreach(Material mat in ren.materials){
                mat.color = toColor;
            }
        }
        if(useRecursion && transform.childCount>0){
            foreach (Transform child in transform) {
                colorRecursive(child, toColor);
            }
        }
    }

    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5

    private static void alphaRecursive( RectTransform rectTransform, float val, int recursiveLevel = 0){
        if(rectTransform.childCount>0){
            foreach (RectTransform child in rectTransform) {
                UnityEngine.UI.Image uiImage = child.GetComponent<UnityEngine.UI.Image>();
                if(uiImage!=null){
                    Color c = uiImage.color;
                    c.a = val;
                    uiImage.color = c;
                }

                alphaRecursive(child, val, recursiveLevel + 1);
            }
        }
    }

    private static void alphaRecursiveSprite( Transform transform, float val ){
        if(transform.childCount>0){
            foreach (Transform child in transform) {
                SpriteRenderer ren = child.GetComponent<SpriteRenderer>();
                if(ren!=null)
                    ren.color = new Color( ren.color.r, ren.color.g, ren.color.b, val);
                alphaRecursiveSprite(child, val);
            }
        }
    }

    private static void colorRecursiveSprite( Transform transform, Color toColor ){
        if(transform.childCount>0){
            foreach (Transform child in transform) {
                SpriteRenderer ren = transform.gameObject.GetComponent<SpriteRenderer>();
                if(ren!=null)
                    ren.color = toColor;
                colorRecursiveSprite(child, toColor);
            }
        }
    }

    private static void colorRecursive( RectTransform rectTransform, Color toColor ){

        if(rectTransform.childCount>0){
            foreach (RectTransform child in rectTransform) {
                UnityEngine.UI.Image uiImage = child.GetComponent<UnityEngine.UI.Image>();
                if(uiImage!=null){
                    uiImage.color = toColor;
                }
                colorRecursive(child, toColor);
            }
        }
    }

    private static void textAlphaRecursive( Transform trans, float val, bool useRecursion = true ){
        UnityEngine.UI.Text uiText = trans.gameObject.GetComponent<UnityEngine.UI.Text>();
        if(uiText!=null){
            Color c = uiText.color;
            c.a = val;
            uiText.color = c;
        }
        if(useRecursion && trans.childCount>0){
            foreach (Transform child in trans) {
                textAlphaRecursive(child, val);
            }
        }
    }

    private static void textColorRecursive(Transform trans, Color toColor ){
        if(trans.childCount>0){
            foreach (Transform child in trans) {
                UnityEngine.UI.Text uiText = child.gameObject.GetComponent<UnityEngine.UI.Text>();
                if(uiText!=null){
                    uiText.color = toColor;
                }
                textColorRecursive(child, toColor);
            }
        }
    }
    #endif

    private static Color tweenColor( LTDescrImpl tween, float val ){
		Vector3 diff3 = tween._optional.point - tween._optional.axis;
        float diffAlpha = tween.to.y - tween.from.y;
		return new Color(tween._optional.axis.x + diff3.x*val, tween._optional.axis.y + diff3.y*val, tween._optional.axis.z + diff3.z*val, tween.from.y + diffAlpha*val);
    }

	/**
	* Pause a tween
	* 
	* @method pause
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/
	public LTDescr pause(){
		if(this.direction != 0.0f){ // check if tween is already paused
        	this.directionLast =  this.direction;
            this.direction = 0.0f;
        }

        return this;
	}

	/**
	* Resume a paused tween
	* 
	* @method resume
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/
	public LTDescr resume(){
		this.direction = this.directionLast;
		
		return this;
	}

	public LTDescr setAxis( Vector3 axis ){
		this._optional.axis = axis;
		return this;
	}
	
	/**
	* Delay the start of a tween
	* 
	* @method setDelay
	* @param {float} float time The time to complete the tween in
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setDelay( 1.5f );
	*/
	public LTDescr setDelay( float delay ){
		if(this.useEstimatedTime){
			this.delay = delay;
		}else{
			this.delay = delay;//*Time.timeScale;
		}
		
		return this;
	}

	/**
	* Set the type of easing used for the tween. <br>
	* <ul><li><a href="LeanTweenType.html">List of all the ease types</a>.</li>
	* <li><a href="http://www.robertpenner.com/easing/easing_demo.html">This page helps visualize the different easing equations</a></li>
	* </ul>
	* 
	* @method setEase
	* @param {LeanTweenType} easeType:LeanTweenType the easing type to use
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
	*/
	public LTDescr setEase( LeanTweenType easeType ){

		switch( easeType ){
			case LeanTweenType.linear:
				val = this.from.x + this.diff.x * ratioPassed; break;
			case LeanTweenType.easeOutQuad:
				val = LeanTween.easeOutQuadOpt(this.from.x, this.diff.x, ratioPassed); break;
			case LeanTweenType.easeInQuad:
				val = LeanTween.easeInQuadOpt(this.from.x, this.diff.x, ratioPassed); break;
			case LeanTweenType.easeInOutQuad:
				setEaseInOutQuad(); break;
			case LeanTweenType.easeInCubic:
				val = LeanTween.easeInCubic(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeOutCubic:
				val = LeanTween.easeOutCubic(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInOutCubic:
				val = LeanTween.easeInOutCubic(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInQuart:
				val = LeanTween.easeInQuart(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeOutQuart:
				val = LeanTween.easeOutQuart(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInOutQuart:
				val = LeanTween.easeInOutQuart(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInQuint:
				val = LeanTween.easeInQuint(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeOutQuint:
				val = LeanTween.easeOutQuint(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInOutQuint:
				val = LeanTween.easeInOutQuint(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInSine:
				val = LeanTween.easeInSine(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeOutSine:
				val = LeanTween.easeOutSine(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInOutSine:
				val = LeanTween.easeInOutSine(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInExpo:
				val = LeanTween.easeInExpo(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeOutExpo:
				val = LeanTween.easeOutExpo(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInOutExpo:
				val = LeanTween.easeInOutExpo(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInCirc:
				val = LeanTween.easeInCirc(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeOutCirc:
				val = LeanTween.easeOutCirc(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInOutCirc:
				val = LeanTween.easeInOutCirc(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInBounce:
				val = LeanTween.easeInBounce(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeOutBounce:
				val = LeanTween.easeOutBounce(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInOutBounce:
				val = LeanTween.easeInOutBounce(this.from.x, this.to.x, ratioPassed); break;
			case LeanTweenType.easeInBack:
				val = LeanTween.easeInBack(this.from.x, this.to.x, ratioPassed, this.overshoot); break;
			case LeanTweenType.easeOutBack:
				val = LeanTween.easeOutBack(this.from.x, this.to.x, ratioPassed, this.overshoot); break;
			case LeanTweenType.easeInOutBack:
				val = LeanTween.easeInOutBack(this.from.x, this.to.x, ratioPassed, this.overshoot); break;
			case LeanTweenType.easeInElastic:
				val = LeanTween.easeInElastic(this.from.x, this.to.x, ratioPassed, this.overshoot, this.period); break;
			case LeanTweenType.easeOutElastic:
				val = LeanTween.easeOutElastic(this.from.x, this.to.x, ratioPassed, this.overshoot, this.period); break;
			case LeanTweenType.easeInOutElastic:
				val = LeanTween.easeInOutElastic(this.from.x, this.to.x, ratioPassed, this.overshoot, this.period); break;
			case LeanTweenType.punch:
			case LeanTweenType.easeShake:
				if(this.tweenType==LeanTweenType.punch){
					this._optional.animationCurve = LeanTween.punch;
				}else if(this.tweenType==LeanTweenType.easeShake){
					this._optional.animationCurve = LeanTween.shake;
				}
				this.toInternal.x = this.from.x + this.to.x;
				this.diff.x = this.to.x - this.from.x;
				val = LeanTween.tweenOnCurve(this, ratioPassed); break;
			case LeanTweenType.easeSpring:
				val = LeanTween.spring(this.from.x, this.to.x, ratioPassed); break;
			default:
				{
					val = this.from.x + this.diff.x * ratioPassed; break;
				}
			}

		return this;
	}

	public LTDescr setEaseInOutQuad(){
		this.tweenType = LeanTweenType.easeInOutQuad;
		this.easeMethod = this.easeInOutQuad;
		return this;
	}

	private Vector3 tweenOnCurve(){
		return	new Vector3(this.from.x + (this.diff.x) * this._optional.animationCurve.Evaluate(ratioPassed),
			this.from.y + (this.diff.y) * this._optional.animationCurve.Evaluate(ratioPassed),
			this.from.z + (this.diff.z) * this._optional.animationCurve.Evaluate(ratioPassed) );
	}

	private Vector3 easeInOutQuad(){
		val = this.ratioPassed * 2f;

		if (val < 1f) {
			val = val * val;
			return new Vector3( this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y, this.diffDiv2.z * val + this.from.z);
		}
		val = (1f-val) * (val - 3f) + 1f;
		return new Vector3( this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y, this.diffDiv2.z * val + this.from.z);
	} 

	private Vector3 easeInQuad(){
		val = ratioPassed * ratioPassed;
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeOutQuad(){
		val = this.ratioPassed * (this.ratioPassed - 2);
		return new Vector3(-this.diff.x * val + this.from.x, -this.diff.y * val + this.from.y, -this.diff.z * val + this.from.z);
	}

	private Vector3 linear(){
		return new Vector3(this.from.x+this.diff.x*this.ratioPassed, this.from.y+this.diff.y*this.ratioPassed, this.from.z+this.diff.z*this.ratioPassed);
	}

	private Vector3 easeInCubic(){
		val = this.ratioPassed * this.ratioPassed * this.ratioPassed;
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeOutCubic(){
		val = this.ratioPassed - 1f;
		val = (val * val * val + 1);
		return new Vector3( this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z) ;
	}

	private Vector3 easeInOutCubic(){
		val = this.ratioPassed * 2f;
		if (val < 1f) {
			val = val * val * val;
			return new Vector3(this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y, this.diffDiv2.z * val + this.from.z);
		}
		val -= 2f;
		val = val * val * val + 2f;
		return new Vector3(this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y,this.diffDiv2.z * val + this.from.z);
	}

	private Vector3 easeOutQuart(){
		val = this.ratioPassed - 1f;
		val = -(val * val * val * val - 1);
		return new Vector3(this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y,this.diffDiv2.z * val + this.from.z);
	}

	private Vector3 easeInOutQuart(){
		val = this.ratioPassed * 2f;
		if (val < 1f) {
			val = val * val * val * val;
			return new Vector3(this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y, this.diffDiv2.z * val + this.from.z);
		}
		val -= 2f;
		val = (val * val * val * val - 2f);
		return new Vector3(-this.diffDiv2.x * val + this.from.x, -this.diffDiv2.x * val + this.from.x, -this.diffDiv2.x * val + this.from.x);
	}

	private Vector3 easeInQuint(){
		val = this.ratioPassed;
		val = val * val * val * val * val;
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeOutQuint(){
		val = this.ratioPassed - 1f;
		val = (val * val * val * val * val + 1f);
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeInOutQuint(){
		val = this.ratioPassed * 2f;
		if (val < 1f){
			val = val * val * val * val * val;
			return new Vector3(this.diffDiv2.x * val + this.from.x,this.diffDiv2.y * val + this.from.y,this.diffDiv2.z * val + this.from.z);
		}
		val -= 2f;
		val = (val * val * val * val * val + 2f);
		return new Vector3(this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y, this.diffDiv2.z * val + this.from.z);
	}

	private Vector3 easeInSine(){
		val = - Mathf.Cos(this.ratioPassed / 1 * LeanTween.PI_DIV2);
		return new Vector3(this.diff.x * val + this.diff.x + this.from.x, this.diff.y * val + this.diff.y + this.from.y, this.diff.z * val + this.diff.z + this.from.z);
	}

	private Vector3 easeOutSine(){
		val = Mathf.Sin(this.ratioPassed / 1 * LeanTween.PI_DIV2);
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y,this.diff.z * val + this.from.z);
	}

	private Vector3 easeInOutSine(){
		val = 2 * (Mathf.Cos(Mathf.PI * this.ratioPassed / 1) - 1);
		return new Vector3(-this.diff.x / val + this.from.x, -this.diff.y / val + this.from.y, -this.diff.z / val + this.from.z);
	}

	private Vector3 easeInExpo(){
		val = Mathf.Pow(2, 10 * (this.ratioPassed / 1 - 1));
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeOutExpo(){
		val = (-Mathf.Pow(2, -10 * this.ratioPassed / 1) + 1);
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeInOutExpo(){
		val = this.ratioPassed * 2f;
		val = Mathf.Pow(2, 10 * (val - 1));
		if (val < 1f){ 
			return new Vector3(this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y, this.diffDiv2.z * val + this.from.z);
		}
		val--;
		val = (-Mathf.Pow(2, -10 * val) + 2);
		return new Vector3(this.diffDiv2.x * val + this.from.x, this.diffDiv2.y * val + this.from.y, this.diffDiv2.z * val + this.from.z);
	}

	private Vector3 easeInCirc(){
		val = -(Mathf.Sqrt(1 - this.ratioPassed * this.ratioPassed) - 1);
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeOutCirc(){
		val = this.ratioPassed - 1f;
		val = Mathf.Sqrt(1 - val * val);
		return new Vector3(this.diff.x * val + this.from.x, this.diff.y * val + this.from.y, this.diff.z * val + this.from.z);
	}

	private Vector3 easeInOutCirc(){
		val = this.ratioPassed * 2f;
		if (val < 1){
			val = 2 * (Mathf.Sqrt(1 - val * val) - 1);
			return new Vector3(-this.diff.x / val  + this.from.x, -this.diff.y / val  + this.from.y, -this.diff.z / val  + this.from.z);
		}
		val -= 2;
		val = 2 * (Mathf.Sqrt(1 - val * val) + 1);
		return new Vector3(this.diff.x / val + this.from.x, this.diff.y / val + this.from.y, this.diff.z / val + this.from.z);
	}

	private Vector3 easeInBounce(){
		val = this.ratioPassed;
		float d = 1f;
		return new Vector3(this.diff.x - LeanTween.easeOutBounce(0, this.diff.x, d-val) + this.from.x, 
			this.diff.y - LeanTween.easeOutBounce(0, this.diff.y, d-val) + this.from.y, 
			this.diff.z - LeanTween.easeOutBounce(0, this.diff.z, d-val) + this.from.z);
	}

	private Vector3 easeOutBounce(){
		val = ratioPassed;
		val /= 1f;
		if (val < (1 / 2.75f)){
			return this.diff * (7.5625f * val * val) + this.from;
		}else if (val < (2 / 2.75f)){
			val -= (1.5f / 2.75f);
			return this.diff * (7.5625f * (val) * val + .75f) + this.from;
		}else if (val < (2.5 / 2.75)){
			val -= (2.25f / 2.75f);
			return this.diff * (7.5625f * (val) * val + .9375f) + this.from;
		}else{
			val -= (2.625f / 2.75f);
			return this.diff * (7.5625f * (val) * val + .984375f) + this.from;
		}
	}

	private Vector3 easeInOutBounce(){
		float d = 1f;
		val = this.ratioPassed * 2f;
		if (val < d/2){
			return new Vector3(LeanTween.easeInBounce(0, this.diff.x, val) * 0.5f + this.from.x, 
				LeanTween.easeInBounce(0, this.diff.y, val) * 0.5f + this.from.y, 
				LeanTween.easeInBounce(0, this.diff.z, val) * 0.5f + this.from.z);
		}else return new Vector3(LeanTween.easeOutBounce(0, this.diff.x, val-d) * 0.5f + this.diffDiv2.x + this.from.x,
			LeanTween.easeOutBounce(0, this.diff.y, val-d) * 0.5f + this.diffDiv2.y + this.from.y,
			LeanTween.easeOutBounce(0, this.diff.z, val-d) * 0.5f + this.diffDiv2.z + this.from.z);
	}

	private Vector3 easeInBack(){
		val = this.ratioPassed;
		val /= 1;
		float s = 1.70158f * this.overshoot;
		return this.diff * (val) * val * ((s + 1) * val - s) + this.from;
	}

	private Vector3 easeOutBack(){
		float s = 1.70158f * this.overshoot;
		val = (this.ratioPassed / 1) - 1;
		val = ((val) * val * ((s + 1) * val + s) + 1);
		return this.diff * val + this.from;
	}

	private Vector3 easeInOutBack(){
		float s = 1.70158f * this.overshoot;
		val = this.ratioPassed * 2f;
		if ((val) < 1){
			s *= (1.525f) * overshoot;
			return this.diffDiv2 * (val * val * (((s) + 1) * val - s)) + this.from;
		}
		val -= 2;
		s *= (1.525f) * overshoot;
		val = ((val) * val * (((s) + 1) * val + s) + 2);
		return this.diffDiv2 * val + this.from;
	}

	private Vector3 easeInElastic(){
		return new Vector3(LeanTween.easeInElastic(this.from.x,this.to.x,this.ratioPassed,this.overshoot,this.period),
			LeanTween.easeInElastic(this.from.y,this.to.y,this.ratioPassed,this.overshoot,this.period),
			LeanTween.easeInElastic(this.from.z,this.to.z,this.ratioPassed,this.overshoot,this.period));
	}		

	private Vector3 easeOutElastic(){
		return new Vector3(LeanTween.easeOutElastic(this.from.x,this.to.x,this.ratioPassed,this.overshoot,this.period),
			LeanTween.easeOutElastic(this.from.y,this.to.y,this.ratioPassed,this.overshoot,this.period),
			LeanTween.easeOutElastic(this.from.z,this.to.z,this.ratioPassed,this.overshoot,this.period));
	}

	private Vector3 easeInOutElastic()
	{
		return new Vector3(LeanTween.easeInOutElastic(this.from.x,this.to.x,this.ratioPassed,this.overshoot,this.period),
			LeanTween.easeInOutElastic(this.from.y,this.to.y,this.ratioPassed,this.overshoot,this.period),
			LeanTween.easeInOutElastic(this.from.z,this.to.z,this.ratioPassed,this.overshoot,this.period));
	}

	/**
	* Set how far past a tween will overshoot  for certain ease types (compatible:  easeInBack, easeInOutBack, easeOutBack, easeOutElastic, easeInElastic, easeInOutElastic). <br>
	* @method setOvershoot
	* @param {float} overshoot:float how far past the destination it will go before settling in
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeOutBack ).setOvershoot(2f);
	*/
	public LTDescr setOvershoot( float overshoot ){
		this.overshoot = overshoot;
		return this;
	}

	/**
	* Set how short the iterations are for certain ease types (compatible: easeOutElastic, easeInElastic, easeInOutElastic). <br>
	* @method setPeriod
	* @param {float} period:float how short the iterations are that the tween will animate at (default 0.3f)
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeOutElastic ).setPeriod(0.3f);
	*/
	public LTDescr setPeriod( float period ){
		this.period = period;
		return this;
	}

	/**
	* Set the type of easing used for the tween with a custom curve. <br>
	* @method setEase (AnimationCurve)
	* @param {AnimationCurve} easeDefinition:AnimationCurve an <a href="http://docs.unity3d.com/Documentation/ScriptReference/AnimationCurve.html" target="_blank">AnimationCure</a> that describes the type of easing you want, this is great for when you want a unique type of movement
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
	*/
	public LTDescr setEase( AnimationCurve easeCurve ){
		this._optional.animationCurve = easeCurve;
		return this;
	}

	/**
	* Set the end that the GameObject is tweening towards
	* @method setTo
	* @param {Vector3} to:Vector3 point at which you want the tween to reach
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LTDescr descr = LeanTween.move( cube, Vector3.up, new Vector3(1f,3f,0f), 1.0f ).setEase( LeanTweenType.easeInOutBounce );<br>
	* // Later your want to change your destination or your destiation is constantly moving<br>
	* descr.setTo( new Vector3(5f,10f,3f); );<br>
	*/
	public LTDescr setTo( Vector3 to ){
		if(this.hasInitiliazed){
			this.to = to;
			this.diff = to - this.from;
		}else{
			this.to = to;
		}
		
		return this;
	}

	public LTDescr setTo( Transform to ){
		this.toTrans = to;
		return this;
	}

	public LTDescr setFrom( Vector3 from ){
		if(this.trans){
			this.init();

		}
		this.from = from;
		// this.hasInitiliazed = true; // this is set, so that the "from" value isn't overwritten later on when the tween starts
		this.diff = this.to - this.from;
		return this;
	}

	public LTDescr setFrom( float from ){
		return setFrom( new Vector3(from, 0f, 0f) );
	}

	public LTDescr setDiff( Vector3 diff ){
		this.diff = diff;
		return this;
	}

	public LTDescr setHasInitialized( bool has ){
		this.hasInitiliazed = has;
		return this;
	}

	public LTDescr setId( uint id ){
		this._id = id;
		this.counter = global_counter;
		// Debug.Log("Global counter:"+global_counter);
		return this;
	}

	/**
	* Set the finish time of the tween
	* @method setTime
	* @param {float} finishTime:float the length of time in seconds you wish the tween to complete in
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* int tweenId = LeanTween.moveX(gameObject, 5f, 2.0f ).id;<br>
	* // Later<br>
	* LTDescr descr = description( tweenId );<br>
	* descr.setTime( 1f );<br>
	*/
	public LTDescr setTime( float time ){
		float passedTimeRatio = this.passed / this.time;
		this.passed = time * passedTimeRatio;
		this.time = time;
		return this;
	}

	/**
	* Set the finish time of the tween
	* @method setSpeed
	* @param {float} speed:float the speed in unity units per second you wish the object to travel (overrides the given time)
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveLocalZ( gameObject, 10f, 1f).setSpeed(0.2f) // the given time is ignored when speed is set<br>
	*/
	public LTDescr setSpeed( float speed ){
		this.speed = speed;
		return this;
	}

	/**
	* Set the tween to repeat a number of times.
	* @method setRepeat
	* @param {int} repeatNum:int the number of times to repeat the tween. -1 to repeat infinite times
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 10 ).setLoopPingPong();
	*/
	public LTDescr setRepeat( int repeat ){
		this.loopCount = repeat;
		if((repeat>1 && this.loopType == LeanTweenType.once) || (repeat < 0 && this.loopType == LeanTweenType.once)){
			this.loopType = LeanTweenType.clamp;
		}
		if(this.type==TweenAction.CALLBACK || this.type==TweenAction.CALLBACK_COLOR){
			this.setOnCompleteOnRepeat(true);
		}
		return this;
	}

	public LTDescr setLoopType( LeanTweenType loopType ){
		this.loopType = loopType;
		return this;
	}

	public LTDescr setUseEstimatedTime( bool useEstimatedTime ){
		this.useEstimatedTime = useEstimatedTime;
		return this;
	}
	
	/**
	* Set ignore time scale when tweening an object when you want the animation to be time-scale independent (ignores the Time.timeScale value). Great for pause screens, when you want all other action to be stopped (or slowed down)
	* @method setIgnoreTimeScale
	* @param {bool} useUnScaledTime:bool whether to use the unscaled time or not
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setIgnoreTimeScale( true );
	*/
	public LTDescr setIgnoreTimeScale( bool useUnScaledTime ){
		this.useEstimatedTime = useUnScaledTime;
		return this;
	}

	/**
	* Use frames when tweening an object, when you don't want the animation to be time-frame independent...
	* @method setUseFrames
	* @param {bool} useFrames:bool whether to use estimated time or not
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setUseFrames( true );
	*/
	public LTDescr setUseFrames( bool useFrames ){
		this.useFrames = useFrames;
		return this;
	}

	public LTDescr setUseManualTime( bool useManualTime ){
		this.useManualTime = useManualTime;
		return this;
	}

	public LTDescr setLoopCount( int loopCount ){
		this.loopType = LeanTweenType.clamp;
		this.loopCount = loopCount;
		return this;
	}

	/**
	* No looping involved, just run once (the default)
	* @method setLoopOnce
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopOnce();
	*/
	public LTDescr setLoopOnce(){ this.loopType = LeanTweenType.once; return this; }

	/**
	* When the animation gets to the end it starts back at where it began
	* @method setLoopClamp
	* @param {int} loops:int (defaults to -1) how many times you want the loop to happen (-1 for an infinite number of times)
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopClamp( 2 );
	*/
	public LTDescr setLoopClamp(){ 
		this.loopType = LeanTweenType.clamp; 
		if(this.loopCount==0)
			this.loopCount = -1;
		return this;
	}
	public LTDescr setLoopClamp( int loops ){ 
		this.loopCount = loops;
		return this;
	}

	/**
	* When the animation gets to the end it then tweens back to where it started (and on, and on)
	* @method setLoopPingPong
	* @param {int} loops:int (defaults to -1) how many times you want the loop to happen in both directions (-1 for an infinite number of times). Passing a value of 1 will cause the object to go towards and back from it's destination once.
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopPingPong( 2 );
	*/
	public LTDescr setLoopPingPong(){
		this.loopType = LeanTweenType.pingPong;
		if(this.loopCount==0)
			this.loopCount = -1;
		return this; 
	}
	public LTDescr setLoopPingPong( int loops ) { 
		this.loopType = LeanTweenType.pingPong;
        this.loopCount = loops == -1 ? loops : loops * 2;
		return this; 
	}

	/**
	* Have a method called when the tween finishes
	* @method setOnComplete
	* @param {Action} onComplete:Action the method that should be called when the tween is finished ex: tweenFinished(){ }
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/
	public LTDescr setOnComplete( Action onComplete ){
		this._optional.onComplete = onComplete;
		return this;
	}

	/**
	* Have a method called when the tween finishes
	* @method setOnComplete (object)
	* @param {Action<object>} onComplete:Action<object> the method that should be called when the tween is finished ex: tweenFinished( object myObj ){ }
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/
	public LTDescr setOnComplete( Action<object> onComplete ){
		this._optional.onCompleteObject = onComplete;
		return this;
	}
	public LTDescr setOnComplete( Action<object> onComplete, object onCompleteParam ){
		this._optional.onCompleteObject = onComplete;
		if(onCompleteParam!=null)
			this._optional.onCompleteParam = onCompleteParam;
		return this;
	}

	/**
	* Pass an object to along with the onComplete Function
	* @method setOnCompleteParam
	* @param {object} onComplete:object an object that 
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.delayedCall(1.5f, enterMiniGameStart).setOnCompleteParam( new object[]{""+5} );<br><br>
	* void enterMiniGameStart( object val ){<br>
    * &nbsp;object[] arr = (object [])val;<br>
    * &nbsp;int lvl = int.Parse((string)arr[0]);<br>
    * }<br>
	*/
	public LTDescr setOnCompleteParam( object onCompleteParam ){
		this._optional.onCompleteParam = onCompleteParam;
		return this;
	}


	/**
	* Have a method called on each frame that the tween is being animated (passes a float value)
	* @method setOnUpdate
	* @param {Action<float>} onUpdate:Action<float> a method that will be called on every frame with the float value of the tweened object
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );<br>
	* <br>
	* void tweenMoved( float val ){ }<br>
	*/
	public LTDescr setOnUpdate( Action<float> onUpdate ){
		this._optional.onUpdateFloat = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}
    public LTDescr setOnUpdateRatio(Action<float,float> onUpdate)
    {
		this._optional.onUpdateFloatRatio = onUpdate;
        this.hasUpdateCallback = true;
        return this;
    }
	
	public LTDescr setOnUpdateObject( Action<float,object> onUpdate ){
		this._optional.onUpdateFloatObject = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}
	public LTDescr setOnUpdateVector2( Action<Vector2> onUpdate ){
		this._optional.onUpdateVector2 = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}
	public LTDescr setOnUpdateVector3( Action<Vector3> onUpdate ){
		this._optional.onUpdateVector3 = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}
	public LTDescr setOnUpdateColor( Action<Color> onUpdate ){
		this._optional.onUpdateColor = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}
	public LTDescr setOnUpdateColor( Action<Color,object> onUpdate ){
		this._optional.onUpdateColorObject = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	#if !UNITY_FLASH

	public LTDescr setOnUpdate( Action<Color> onUpdate ){
		this._optional.onUpdateColor = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate( Action<Color,object> onUpdate ){
		this._optional.onUpdateColorObject = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	/**
	* Have a method called on each frame that the tween is being animated (passes a float value and a object)
	* @method setOnUpdate (object)
	* @param {Action<float,object>} onUpdate:Action<float,object> a method that will be called on every frame with the float value of the tweened object, and an object of the person's choosing
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );<br>
	* <br>
	* void tweenMoved( float val, object obj ){ }<br>
	*/
	public LTDescr setOnUpdate( Action<float,object> onUpdate, object onUpdateParam = null ){
		this._optional.onUpdateFloatObject = onUpdate;
		this.hasUpdateCallback = true;
		if(onUpdateParam!=null)
			this._optional.onUpdateParam = onUpdateParam;
		return this;
	}

	public LTDescr setOnUpdate( Action<Vector3,object> onUpdate, object onUpdateParam = null ){
		this._optional.onUpdateVector3Object = onUpdate;
		this.hasUpdateCallback = true;
		if(onUpdateParam!=null)
			this._optional.onUpdateParam = onUpdateParam;
		return this;
	}

	public LTDescr setOnUpdate( Action<Vector2> onUpdate, object onUpdateParam = null ){
		this._optional.onUpdateVector2 = onUpdate;
		this.hasUpdateCallback = true;
		if(onUpdateParam!=null)
			this._optional.onUpdateParam = onUpdateParam;
		return this;
	}

	/**
	* Have a method called on each frame that the tween is being animated (passes a float value)
	* @method setOnUpdate (Vector3)
	* @param {Action<Vector3>} onUpdate:Action<Vector3> a method that will be called on every frame with the float value of the tweened object
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );<br>
	* <br>
	* void tweenMoved( Vector3 val ){ }<br>
	*/
	public LTDescr setOnUpdate( Action<Vector3> onUpdate, object onUpdateParam = null ){
		this._optional.onUpdateVector3 = onUpdate;
		this.hasUpdateCallback = true;
		if(onUpdateParam!=null)
			this._optional.onUpdateParam = onUpdateParam;
		return this;
	}
	#endif
	

	/**
	* Have an object passed along with the onUpdate method
	* @method setOnUpdateParam
	* @param {object} onUpdateParam:object an object that will be passed along with the onUpdate method
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );<br>
	* <br>
	* void tweenMoved( float val, object obj ){ }<br>
	*/
	public LTDescr setOnUpdateParam( object onUpdateParam ){
		this._optional.onUpdateParam = onUpdateParam;
		return this;
	}

	/**
	* While tweening along a curve, set this property to true, to be perpendicalur to the path it is moving upon
	* @method setOrientToPath
	* @param {bool} doesOrient:bool whether the gameobject will orient to the path it is animating along
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true).setAxis(Vector3.forward);<br>
	*/
	public LTDescr setOrientToPath( bool doesOrient ){
		if(this.type==TweenAction.MOVE_CURVED || this.type==TweenAction.MOVE_CURVED_LOCAL){
			if(this._optional.path==null)
				this._optional.path = new LTBezierPath();
			this._optional.path.orientToPath = doesOrient;
		}else{
			this._optional.spline.orientToPath = doesOrient;
		}
		return this;
	}

	/**
	* While tweening along a curve, set this property to true, to be perpendicalur to the path it is moving upon
	* @method setOrientToPath2d
	* @param {bool} doesOrient:bool whether the gameobject will orient to the path it is animating along
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath2d(true).setAxis(Vector3.forward);<br>
	*/
	public LTDescr setOrientToPath2d( bool doesOrient2d ){
		setOrientToPath(doesOrient2d);
		if(this.type==TweenAction.MOVE_CURVED || this.type==TweenAction.MOVE_CURVED_LOCAL){
			this._optional.path.orientToPath2d = doesOrient2d;
		}else{
			this._optional.spline.orientToPath2d = doesOrient2d;
		}
		return this;
	}

	public LTDescr setRect( LTRect rect ){
		this._optional.ltRect = rect;
		return this;
	}

	public LTDescr setRect( Rect rect ){
		this._optional.ltRect = new LTRect(rect);
		return this;
	}

	public LTDescr setPath( LTBezierPath path ){
		this._optional.path = path;
		return this;
	}

	/**
	* Set the point at which the GameObject will be rotated around
	* @method setPoint
	* @param {Vector3} point:Vector3 point at which you want the object to rotate around (local space)
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.rotateAround( cube, Vector3.up, 360.0f, 1.0f ) .setPoint( new Vector3(1f,0f,0f) ) .setEase( LeanTweenType.easeInOutBounce );<br>
	*/
	public LTDescr setPoint( Vector3 point ){
		this._optional.point = point;
		return this;
	}

	public LTDescr setDestroyOnComplete( bool doesDestroy ){
		this.destroyOnComplete = doesDestroy;
		return this;
	}

	public LTDescr setAudio( object audio ){
		this._optional.onCompleteParam = audio;
		return this;
	}
	
	/**
	* Set the onComplete method to be called at the end of every loop cycle (also applies to the delayedCall method)
	* @method setOnCompleteOnRepeat
	* @param {bool} isOn:bool does call onComplete on every loop cycle
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.delayedCall(gameObject,0.3f, delayedMethod).setRepeat(4).setOnCompleteOnRepeat(true);
	*/
	public LTDescr setOnCompleteOnRepeat( bool isOn ){
		this.onCompleteOnRepeat = isOn;
		return this;
	}

	/**
	* Set the onComplete method to be called at the beginning of the tween (it will still be called when it is completed as well)
	* @method setOnCompleteOnStart
	* @param {bool} isOn:bool does call onComplete at the start of the tween
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.delayedCall(gameObject, 2f, ()=>{<br> // Flash an object 5 times
	* &nbsp;LeanTween.alpha(gameObject, 0f, 1f);<br>
	* &nbsp;LeanTween.alpha(gameObject, 1f, 0f).setDelay(1f);<br>
	* }).setOnCompleteOnStart(true).setRepeat(5);<br>
	*/
	public LTDescr setOnCompleteOnStart( bool isOn ){
		this.onCompleteOnStart = isOn;
		return this;
	}

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
	public LTDescr setRect( RectTransform rect ){
		this.rectTransform = rect;
		return this;
	}

	public LTDescr setSprites( UnityEngine.Sprite[] sprites ){
		this.sprites = sprites;
		return this;
	}

	public LTDescr setFrameRate( float frameRate ){
		this.time = this.sprites.Length / frameRate;
		return this;
	}
#endif
	
	/**
	* Have a method called when the tween starts
	* @method setOnStart
	* @param {Action<>} onStart:Action<> the method that should be called when the tween is starting ex: tweenStarted( ){ }
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* <i>C#:</i><br>
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnStart( ()=>{ Debug.Log("I started!"); });
	* <i>Javascript:</i><br>
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnStart( function(){ Debug.Log("I started!"); } );
	*/
	public LTDescr setOnStart( Action onStart ){
		this._optional.onStart = onStart;
        return this;
    }

    /**
	* Set the direction of a tween -1f for backwards 1f for forwards (currently only bezier and spline paths are supported)
	* @method setDirection
	* @param {float} direction:float the direction that the tween should run, -1f for backwards 1f for forwards
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
    * LeanTween.moveSpline(gameObject, new Vector3[]{new Vector3(0f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,1f)}, 1.5f).setDirection(-1f);<br>
	*/

    public LTDescr setDirection( float direction ){
    	if(this.direction!=-1f && this.direction!=1f){
    		Debug.LogWarning("You have passed an incorrect direction of '"+direction+"', direction must be -1f or 1f");
    		return this;
    	}

    	if(this.direction!=direction){
	    	// Debug.Log("reverse path:"+this.path+" spline:"+this._optional.spline+" hasInitiliazed:"+this.hasInitiliazed);
	    	if(this.hasInitiliazed){
	    		this.direction = direction;
    		}else{
				if(this._optional.path!=null){
					this._optional.path = new LTBezierPath( LTUtility.reverse( this._optional.path.pts ) );
				}else if(this._optional.spline!=null){
					this._optional.spline = new LTSpline( LTUtility.reverse( this._optional.spline.pts ) );
				}
				// this.passed = this.time - this.passed;
    		}
		}
    	
    	return this;
    }

    /**
	* Set whether or not the tween will recursively effect an objects children in the hierarchy
	* @method setRecursive
	* @param {bool} useRecursion:bool whether the tween will recursively effect an objects children in the hierarchy
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
    * LeanTween.alpha(gameObject, 0f, 1f).setRecursive(true);<br>
	*/

    public LTDescr setRecursive( bool useRecursion ){
    	this.useRecursion = useRecursion;
    	
    	return this;
    }
}
