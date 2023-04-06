using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISettableObject
{
    /// <summary>
    /// 오브젝트에 아이템을 세팅할 수 있는 함수
    /// </summary>
    /// <param name="data">세팅할 아이템</param>
    /// <returns>아이템 사용 후 인벤토리에서 사라지면 true 반환, 그대로 있으면 false 반환</returns>
    bool Set(ItemData data);
}
