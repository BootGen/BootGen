

interface EntityWithId {
  id: number;
}

export function setItem<T extends EntityWithId>(array: Array<T>, item: T): T {
  const oldItem = array.find(i => i.id == item.id);
  if (oldItem) {
    Object.assign(oldItem, item);
    return oldItem;
  } else {
    array.push(item);
    return item;
  }
}

export function setArray<T extends EntityWithId>(target: Array<T>, source: Array<T>): Array<T> {
  const result = Array<T>();
  source.forEach(item =>{
    const oldItem = target.find(i => i.id == item.id);
    if (oldItem) {
      Object.assign(oldItem, item);
      result.push(oldItem);
    } else {
      result.push(item);
    }
  });
  return result;
}

export function patchArray<T extends EntityWithId>(target: Array<T>, source: Array<T>) {
  source.forEach(item =>{
    setItem(target, item);
  });
}

export function findById<T extends EntityWithId>(array: Array<T>, id: number): T | undefined {
  return array.find(i => i.id === id);
}

export function findObjectsById<T extends EntityWithId>(lookupArray: Array<T>, dataArray: Array<T>): Array<T> {
  const result = Array<T>();
  dataArray.forEach((item: T) => {
    const obj = findById(lookupArray, item.id);
    if (obj) {
      result.push(obj);
    }
  });
  return result;
}
