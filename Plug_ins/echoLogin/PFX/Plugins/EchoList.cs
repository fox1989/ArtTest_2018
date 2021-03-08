// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


//-----------------------------------------------------------------------------
public class EchoList<T> where T: class, new()
{
	public EchoListItem<T> 				first_on;
	EchoListItem<T> 					last_on;
	EchoListItem<T> 					first_off;
	EchoListItem<T> 					last_off;
	Dictionary< T, EchoListItem<T> > 	myDict = new Dictionary<T, EchoListItem<T>>();

	public EchoList ( int isize )
	{
		first_on 	= new EchoListItem<T>();
		first_off 	= new EchoListItem<T>();
		last_on 	= new EchoListItem<T>();
		last_off 	= new EchoListItem<T>();

		first_on.next = last_on;
		first_on.prev = null;
		
		last_on.prev = first_on;
		last_on.next = null;
		
		first_off.next = last_off;
		first_off.prev = null;

		last_off.prev = first_off;
		last_off.next = null;
		
		for ( int loop = 0; loop < isize; loop++ )
		{
			AddNewItem();
		}
	}

	public EchoList ( T[] itemArray )
	{
		first_on 	= new EchoListItem<T>();
		first_off 	= new EchoListItem<T>();
		last_on 	= new EchoListItem<T>();
		last_off 	= new EchoListItem<T>();

		first_on.next = last_on;
		first_on.prev = null;
		
		last_on.prev = first_on;
		last_on.next = null;
		
		first_off.next = last_off;
		first_off.prev = null;

		last_off.prev = first_off;
		last_off.next = null;
		
		for ( int loop = 0; loop < itemArray.Length; loop++ )
		{
			AddNewItem(itemArray[loop]);
		}
	}
	
	public EchoList ( List<T> itemArray )
	{
		first_on 	= new EchoListItem<T>();
		first_off 	= new EchoListItem<T>();
		last_on 	= new EchoListItem<T>();
		last_off 	= new EchoListItem<T>();

		first_on.next = last_on;
		first_on.prev = null;
		
		last_on.prev = first_on;
		last_on.next = null;
		
		first_off.next = last_off;
		first_off.prev = null;

		last_off.prev = first_off;
		last_off.next = null;
		
		for ( int loop = 0; loop < itemArray.Count; loop++ )
		{
			AddNewItem(itemArray[loop]);
		}
	}

	//========================
	private void AddNewItem ()
	{
		EchoListItem<T> item = new EchoListItem<T>( new T() );
		
		item.next  			= last_off;
		item.prev  			= last_off.prev;
		last_off.prev.next 	= item;
		last_off.prev       = item;
		
		myDict.Add ( item.item, item );
	}

	//========================
	private void AddNewItem ( T in_item )
	{
		EchoListItem<T> item = new EchoListItem<T>( in_item );
		
		item.next  			= last_off;
		item.prev  			= last_off.prev;
		last_off.prev.next 	= item;
		last_off.prev       = item;
		
		myDict.Add ( item.item, item );
	}

	//=========================================================================
	public T GetFreeDynamicAdd()
	{
		EchoListItem<T> item = first_off.next;
		
		if ( item == null )
			AddNewItem();

		if ( item != null)
			Inactive2Active ( item );
		
		return ( item.item );
	}

	//=========================================================================
	public T GetFree()
	{
		EchoListItem<T> item = first_off.next;
		
		if ( item != null )
			Inactive2Active ( item );
		
		return ( item.item );
	}

	//=========================================================================
	public void Activate ( T item )
	{
		Inactive2Active ( myDict[item] );
	}

	//=========================================================================
	public void Deactivate ( T item )
	{
		Active2Inactive ( myDict[item] );
	}
	
	//=========================================================================
	public T GetNext ( T item )
	{
		return ( myDict[item].next.item );
	}
	
	//=========================================================================
	public T GetFirstActive ()
	{
		return ( first_on.next.item );
	}

	//=========================================================================
	public T GetFirstInactive ()
	{
		return ( first_off.next.item );
	}

	//=========================================================================
	public void Active2Inactive ( EchoListItem<T> in_item )
	{
		// remove from current list;
		in_item.prev.next = in_item.next; 	
		in_item.next.prev = in_item.prev;
		
		//Add to inactive list
		in_item.next 			= last_off;
		in_item.prev 			= last_off.prev;
		last_off.prev.next 		= in_item;
		last_off.prev       	= in_item;
	}

	//=========================================================================
	public void Inactive2Active ( EchoListItem<T> in_item )
	{
		// remove from current list;
		in_item.prev.next = in_item.next; 	
		in_item.next.prev = in_item.prev;
		
		//Add to active list
		in_item.next 			= last_on;
		in_item.prev 			= last_on.prev;
		last_on.prev.next 		= in_item;
		last_on.prev         	= in_item;
	}

}

//-----------------------------------------------------------------------------
public class EchoListItem<T>
{
	public EchoListItem<T> next;
	public EchoListItem<T> prev;
	public T item;

	//=========================================================================
	public EchoListItem ()
	{
		item = default(T);
	}

	//=========================================================================
	public EchoListItem ( T i_item )
	{
		item = i_item;
	}
}


