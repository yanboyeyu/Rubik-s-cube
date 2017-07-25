///本脚本为单例脚本
///调用 Rotation(scriptRotationController,dir)函数 可让魔方按传递参数所指旋转
///前一个参数表示旋转层为枚举类型ScriptRotationController例如scriptRotationController=ScriptRotationController.rotation_x_1
///后一个参数为bool类型，表示旋转方向
///用RotationMessage类型接收TouchRotation（）函数的返回值，可以判断触屏移动魔方的相关参数
///RotationMessage类型含有两个参数，分别为ScriptRotationController类型和bool类型，跟Rotation（）函数所需参数相同。


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScriptRotationController{
    rotation_x_1,
    rotation_x_2,
    rotation_x_3,
    rotation_y_1,
    rotation_y_2,
    rotation_y_3,
    rotation_z_1,
    rotation_z_2,
    rotation_z_3
}
public class RotationMessage {
    public ScriptRotationController scriptRotationController;
    public bool dir;
}

public class mofang : MonoBehaviour{

    public static mofang _intance;
    public ScriptRotationController scriptRotationController = ScriptRotationController.rotation_x_1;//测试完删除
    public bool dir = true;//测试完删除
    public RotationMessage rotationMessage=new RotationMessage();
    private void Awake()
    {
        _intance = this;
    }

    public GameObject AnchorPoint;
    private GameObject rateWasGameObject;//被点中物体//
    private Vector3 mouseButtonDownhitPoint;//获取点击碰撞点//
    private Vector3 mouseButtonhitPoint;//获取按键碰撞点//
    private bool isChangParent = false;//是否已经改变父对象//
    private Vector3 mouseDownPoint;//鼠标点击位置//
    private Vector3 mouseButtonPoint;//鼠标拖动位置//
    private Vector3 collideNormal;//碰撞法线//
    private bool isReset = false;//是否已经复原//

    private float smoothing = 5;
    private bool isTween = false;//是否是转动状态//
    private Quaternion targetAngle;//魔方转动终点//
    private int moveFrame = 0;//魔方转动帧数的计数//

    void Update() {
        if (!isTween&& isChangParent && !isReset)
        {//复位//
            AnchorPointReset();
        }
        if (Input.GetMouseButtonDown(0))
        {//点中的Cube
            CollideCube();
        }
        if (Input.GetMouseButtonUp(0))
        {//当鼠标弹起碰撞物体为空//
            rateWasGameObject = null;
        }
        if (Input.GetMouseButton(0) && rateWasGameObject != null)
        {
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            if (Physics.Raycast(ray1, out hit1))                //发出碰撞射线//
            {
                mouseButtonhitPoint = hit1.point;       //获取碰撞点//
            }
            if(!isChangParent && (Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5 || Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5))
            {
                RotationMessage message = TouchRotation(); //测试完用下一条
                //TouchRotation(); 
                print(message.dir);//测试完删除
                print(message.scriptRotationController);//测试完删除
            }            
        }
        //测试完删除
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rotation(scriptRotationController, dir);
        }
    }
    private void LateUpdate()
    {
        if (isTween)
        {
            MofangMove();
        }
    }
    void MofangMove(){
        AnchorPoint.transform.rotation = Quaternion.Lerp(AnchorPoint.transform.rotation, targetAngle,smoothing* Time.deltaTime*1.4f);
        moveFrame++;
        if (moveFrame==(int)60/smoothing){
            AnchorPoint.transform.rotation = targetAngle;
            targetAngle = Quaternion.Euler(0,0,0);
            isTween = false;
            moveFrame = 0;
        }
    }
    void AnchorPointReset()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject go in gos)
        {
            go.transform.parent = transform;
        }
        AnchorPoint.transform.rotation = Quaternion.identity;
        isReset = true;
        collideNormal = new Vector3(0, 0, 0);
        isChangParent = false;
    }
    void CollideCube() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))              //发出碰撞射线//
        {
            rateWasGameObject = hit.collider.gameObject;//获取碰撞物体//
            mouseButtonDownhitPoint = hit.point;    //获取鼠标点击碰撞点//
            mouseDownPoint = Input.mousePosition;   //获取鼠标点击位置//
            collideNormal = hit.normal;//获取碰撞法线//
        }
    }
    public RotationMessage TouchRotation(){
        //RotationMessage rotationMessage=new RotationMessage();
        if (Mathf.Abs(collideNormal.z) > 0.5)
        {//前后两面//
            if ((Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5 || Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5) && !isChangParent)
            {//拖动鼠标超过0.5//
                GameObject[] gos;
                gos = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos)
                {
                    if (Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5)
                    {
                        if (Mathf.Abs(go.transform.position.y - rateWasGameObject.transform.position.y) < 0.2)
                        {
                            go.transform.parent = AnchorPoint.transform;
                        }
                    }
                    else if (Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5)
                    {
                        if (Mathf.Abs(go.transform.position.x - rateWasGameObject.transform.position.x) < 0.2)
                        {
                            go.transform.parent = AnchorPoint.transform;
                        }
                    }
                }
                mouseButtonPoint = Input.mousePosition;//获取鼠标最后位置//
                if (Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5)
                {
                    Vector3 pos = rateWasGameObject.transform.position;
                    if(pos.y>-1.2&&pos.y<-0.8){
                        this.rotationMessage.scriptRotationController = ScriptRotationController.rotation_y_1;
                    }else if(pos.y > -0.2 && pos.y <0.2)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_y_2;
                    }else{
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_y_3;
                    }
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    if ((mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0)
                    {
                        rotationMessage.dir = true;
                        targetAngle = Quaternion.Euler(angles.x, angles.y - 90, angles.z);
                        isTween = true;
                    }
                    else
                    {
                        rotationMessage.dir = false;
                        targetAngle = Quaternion.Euler(angles.x, angles.y + 90, angles.z);
                        isTween = true;
                    }
                }
                else if (Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5)
                {
                    Vector3 pos = rateWasGameObject.transform.position;
                    if (pos.x > -1.2 && pos.x < -0.8)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_x_1;
                    }
                    else if (pos.x > -0.2 && pos.x < 0.2)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_x_2;
                    }
                    else
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_x_3;
                    }
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    if ((mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0)
                    {
                        rotationMessage.dir = false;
                        targetAngle = Quaternion.Euler(angles.x + 90, angles.y, angles.z);
                        isTween = true;                        
                    }
                    else
                    {
                        rotationMessage.dir = true;
                        targetAngle = Quaternion.Euler(angles.x - 90, angles.y, angles.z);
                        isTween = true;
                    }
                }
                isChangParent = true;
                isReset = false;
            }
        }
        if (Mathf.Abs(collideNormal.x) > 0.5)
        {//左右两面//
            if ((Mathf.Abs(mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0.5 || Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5) && !isChangParent)
            {//拖动鼠标超过0.5//
                GameObject[] gos2;
                gos2 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go2 in gos2)
                {
                    if (Mathf.Abs(mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0.5)
                    {
                        if (Mathf.Abs(go2.transform.position.y - rateWasGameObject.transform.position.y) < 0.2)
                        {
                            go2.transform.parent = AnchorPoint.transform;
                        }
                    }
                    else if (Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5)
                    {
                        if (Mathf.Abs(go2.transform.position.z - rateWasGameObject.transform.position.z) < 0.2)
                        {
                            go2.transform.parent = AnchorPoint.transform;
                        }
                    }
                }
                mouseButtonPoint = Input.mousePosition;//获取鼠标最后位置//
                if (Mathf.Abs(mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0.5)
                {
                    Vector3 pos = rateWasGameObject.transform.position;
                    if (pos.y > -1.2 && pos.y < -0.8)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_y_1;
                    }
                    else if (pos.x > -0.2 && pos.x < 0.2)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_y_2;
                    }
                    else
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_y_3;
                    }
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    if ((mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0)
                    {
                        rotationMessage.dir = true;
                        targetAngle = Quaternion.Euler(angles.x, angles.y - 90, angles.z);
                        isTween = true;
                    }
                    else
                    {
                        rotationMessage.dir = false; 
                        targetAngle = Quaternion.Euler(angles.x, angles.y + 90, angles.z);
                        isTween = true;
                    }
                }
                else if (Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5)
                {
                    Vector3 pos = rateWasGameObject.transform.position;
                    if (pos.z > -1.2 && pos.z < -0.8)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_z_1;
                    }
                    else if (pos.z > -0.2 && pos.z < 0.2)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_z_2;
                    }
                    else
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_z_3;
                    }
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    if ((mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0)
                    {
                        rotationMessage.dir = false;
                        targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z + 90);
                        isTween = true;
                    }
                    else
                    {
                        rotationMessage.dir = true;
                        targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z - 90);
                        isTween = true;
                    }
                }
                isChangParent = true;
                isReset = false;
            }
        }
        if (Mathf.Abs(collideNormal.y) > 0.5)
        {//上下两面//
            if ((Mathf.Abs(mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0.5 || Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5) && !isChangParent)
            {//拖动鼠标超过0.5//
                GameObject[] gos3;
                gos3 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go3 in gos3)
                {
                    if (Mathf.Abs(mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0.5)
                    {
                        if (Mathf.Abs(go3.transform.position.x - rateWasGameObject.transform.position.x) < 0.2)
                        {
                            go3.transform.parent = AnchorPoint.transform;
                        }
                    }
                    else if (Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5)
                    {
                        if (Mathf.Abs(go3.transform.position.z - rateWasGameObject.transform.position.z) < 0.2)
                        {
                            go3.transform.parent = AnchorPoint.transform;
                        }
                    }
                }
                mouseButtonPoint = Input.mousePosition;//获取鼠标最后位置//
                if (Mathf.Abs(mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0.5)
                {
                    Vector3 pos = rateWasGameObject.transform.position;
                    if (pos.x > -1.2 && pos.x < -0.8)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_x_1;
                    }
                    else if (pos.x> -0.2 && pos.x < 0.2)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_x_2;
                    }
                    else
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_x_3;
                    }
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    if ((mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0)
                    {
                        rotationMessage.dir = false;
                        targetAngle = Quaternion.Euler(angles.x + 90, angles.y, angles.z);
                        isTween = true;
                    }
                    else
                    {
                        rotationMessage.dir = true;
                        targetAngle = Quaternion.Euler(angles.x - 90, angles.y, angles.z);
                        isTween = true;
                    }
                }
                else if (Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5)
                {
                    Vector3 pos = rateWasGameObject.transform.position;
                    if (pos.z > -1.2 && pos.z < -0.8)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_z_1;
                    }
                    else if (pos.z > -0.2 && pos.z < 0.2)
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_z_2;
                    }
                    else
                    {
                        rotationMessage.scriptRotationController = ScriptRotationController.rotation_z_3;
                    }
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    if ((mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0)
                    {
                        rotationMessage.dir = true;
                        targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z - 90);
                        isTween = true;
                    }
                    else
                    {
                        rotationMessage.dir = false;
                        targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z + 90);
                        isTween = true;
                    }
                }
                isChangParent = true;
                isReset = false;
            }
        }
        return rotationMessage;
    }   
    public void Rotation(ScriptRotationController scriptRotationController,bool dir)
    {
        switch (scriptRotationController)
        {
            case ScriptRotationController.rotation_x_1:
                GameObject[] gos1;
                gos1 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos1){
                    Vector3 pos = go.transform.position;
                    if (pos.x<-0.8f&&pos.x>-1.2f){
                        go.transform.parent = AnchorPoint.transform;
                    }                    
                }
                if(dir){
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x-90, angles.y, angles.z);
                }else{
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x+90, angles.y, angles.z);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_x_2:
                GameObject[] gos2;
                gos2 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos2)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.x< 0.2f && pos.x>-0.2f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x - 90, angles.y, angles.z);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x + 90, angles.y, angles.z);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_x_3:
                GameObject[] gos3;
                gos3 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos3)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.x < 1.2f && pos.x >0.8f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x - 90, angles.y, angles.z);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x + 90, angles.y, angles.z);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_y_1:
                GameObject[] gos4;
                gos4 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos4)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.y <-0.8f && pos.y >-1.2f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y - 90, angles.z);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y + 90, angles.z);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_y_2:
                GameObject[] gos5;
                gos5 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos5)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.y < 0.2f && pos.y > -0.2f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y - 90, angles.z);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y + 90, angles.z);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_y_3:
                GameObject[] gos6;
                gos6 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos6)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.y < 1.2f && pos.y > 0.8f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y - 90, angles.z);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y + 90, angles.z);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_z_1:
                GameObject[] gos7;
                gos7 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos7)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.z < -0.8f && pos.z > -1.2f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z - 90);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z + 90);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_z_2:
                GameObject[] gos8;
                gos8 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos8)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.z < 0.2f && pos.z > -0.2f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z - 90);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z + 90);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            case ScriptRotationController.rotation_z_3:
                GameObject[] gos9;
                gos9 = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject go in gos9)
                {
                    Vector3 pos = go.transform.position;
                    if (pos.z < 1.2f && pos.z >0.8f)
                    {
                        go.transform.parent = AnchorPoint.transform;
                    }
                }
                if (dir)
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z - 90);
                }
                else
                {
                    Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                    targetAngle = Quaternion.Euler(angles.x, angles.y, angles.z + 90);
                }
                isTween = true;
                isChangParent = true;
                isReset = false;
                break;
            default:
                break;
        }
    }

}
