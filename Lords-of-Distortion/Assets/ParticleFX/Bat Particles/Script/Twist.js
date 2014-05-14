var twist:float = 10;


function Update () {
transform.Rotate(Vector3.back*twist*Time.deltaTime);
}