using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;

public delegate void StoreLoaded();
public class GlobalStorage
{
    private WorldAnchorStore store = null;

    private static GlobalStorage instance = null;

    public event StoreLoaded OnStoreLoaded;

    protected GlobalStorage()
    {
        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    public static GlobalStorage Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GlobalStorage();
            }

            return instance;
        }
    }

    public WorldAnchorStore Store
    {
        get
        {
            return store;
        }
    }

    public void AnchorStoreReady(WorldAnchorStore store)
    {
        this.store = store;

        if (OnStoreLoaded != null)
        {
            OnStoreLoaded();
        }
    }
}