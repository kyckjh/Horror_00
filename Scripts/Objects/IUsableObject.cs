using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IUsableObject
{
    /// <summary>
    /// 사용 가능한 오브젝트
    /// </summary>
    /// <param name="data">오브젝트에 사용할 아이템</param>
    /// <returns>아이템 사용 후 인벤토리에서 사라지면 true 반환, 그대로 있으면 false 반환</returns>
    bool Use(ItemData data);
}
