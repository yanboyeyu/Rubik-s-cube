using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mofang : MonoBehaviour {

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
            var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            if (Physics.Raycast(ray1, out hit1))                //发出碰撞射线//
            {
                mouseButtonhitPoint = hit1.point;       //获取碰撞点//
            }
            if (Mathf.Abs(collideNormal.z) > 0.5)
            {//前后两面//
                FrontOrBackMove();
            }

            if (Mathf.Abs(collideNormal.x) > 0.5)
            {//左右两面//
                LeftOrRightMove();
            }
            if (Mathf.Abs(collideNormal.y) > 0.5)
            {//上下两面//
                UpOrDownMove();
            }
        }
       
    }
    private void FixedUpdate()
    {
        if (isTween)
        {
            MofangMove();
        }
    }

    void MofangMove(){
        AnchorPoint.transform.rotation = Quaternion.Lerp(AnchorPoint.transform.rotation, targetAngle,smoothing* Time.deltaTime*1.4f);
        print(AnchorPoint.transform.eulerAngles);
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
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))              //发出碰撞射线//
        {
            rateWasGameObject = hit.collider.gameObject;//获取碰撞物体//
            mouseButtonDownhitPoint = hit.point;    //获取鼠标点击碰撞点//
            mouseDownPoint = Input.mousePosition;   //获取鼠标点击位置//
            collideNormal = hit.normal;//获取碰撞法线//
        }
    }
    void FrontOrBackMove(){
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
            if (Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5 )
            {
                Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                if ((mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0){
                    targetAngle = Quaternion.Euler(angles.x, angles .y- 90, angles.z);
                    isTween = true;
                }
                else{
                    targetAngle = Quaternion.Euler(angles.x, angles.y+ 90, angles.z);
                    isTween = true;
                }
            }
            else if (Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5)
            {
                Quaternion angles = Quaternion.Euler( AnchorPoint.transform.eulerAngles);
                if ((mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0)
                {
                    targetAngle = Quaternion.Euler(angles.x+90, angles.y, angles.z);
                    isTween = true;
                }
                else
                {
                    targetAngle = Quaternion.Euler(angles.x-90, angles.y, angles.z);
                    isTween = true;
                } 
            }
            isChangParent = true;
            isReset = false;
        }
    }
    void LeftOrRightMove(){
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
                Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                if ((mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0)
                {
                    targetAngle = Quaternion.Euler(angles.x, angles.y- 90, angles.z);
                    isTween = true;
                }
                else
                {
                    targetAngle = Quaternion.Euler(angles.x, angles.y + 90, angles.z);
                    isTween = true;
                }
            }
            else if (Mathf.Abs(mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0.5)
            {
                Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                if ((mouseButtonhitPoint.y - mouseButtonDownhitPoint.y) > 0){
                    targetAngle = Quaternion.Euler(angles.x, angles.y , angles.z+90);
                    isTween = true;
                }
                else
                {
                    targetAngle = Quaternion.Euler(angles.x, angles.y , angles.z+90);
                    isTween = true;
                }
            }
            isChangParent = true;
            isReset = false;
        }
    }
    void UpOrDownMove(){
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
                Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                if ((mouseButtonhitPoint.z - mouseButtonDownhitPoint.z) > 0)
                {
                    targetAngle = Quaternion.Euler(angles.x+90, angles.y, angles.z);
                    isTween = true;
                }
                else
                {
                    targetAngle = Quaternion.Euler(angles.x-90, angles.y, angles.z);
                    isTween = true;
                }
            }
            else if (Mathf.Abs(mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0.5)
            {
                Quaternion angles = Quaternion.Euler(AnchorPoint.transform.eulerAngles);
                if ((mouseButtonhitPoint.x - mouseButtonDownhitPoint.x) > 0)
                {
                    targetAngle = Quaternion.Euler(angles.x, angles.y , angles.z-90);
                    isTween = true;
                }
                else
                {
                    targetAngle = Quaternion.Euler(angles.x, angles.y , angles.z + 90);
                    isTween = true;
                }
            }
            isChangParent = true;
            isReset = false;
        }
    }
}
