using System.Collections;

public interface ICollectable
{
    void OnCollect();
    IEnumerator OnCollectCo();
}