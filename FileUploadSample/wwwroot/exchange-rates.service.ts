import { Injectable, OnInit } from '@angular/core';
import { AngularFirestoreCollection, AngularFirestore } from 'angularfire2/firestore';
import * as firebase from 'firebase/app';
// import { WhereFilterOp } from 'firebase/app';
// import { WhereFilterOp } from '@google-cloud/firestore';

interface IExchangeRate {
    createdDate: any;
    rate: number;
}

@Injectable()
export class ExchangeRatesService implements OnInit {

    constructor(private afs: AngularFirestore) { }

    ngOnInit(): void {
        throw new Error('Method not implemented.');
    }

    // operation: 'next' -get next page; 'previous' - get previous page.
    getRates = (date: Date, operation: string = 'next') => {

        let operator: firebase.firestore.WhereFilterOp = '<';
        date = date || new Date();

        if (operation === 'previous') {
            operator = '>';
        }

        return this.afs.collection<IExchangeRate>('exchangeRates',
            ref => ref.where('createdDate', operator, date).orderBy('createdDate', 'desc').limit(10)).valueChanges();
            // ref => ref.orderBy('createdDate', 'desc').limit(5)).valueChanges();
    }

    getNextPage = (date: Date) => {
        date = date || new Date();
        return this.afs.collection<IExchangeRate>('exchangeRates',
            ref => ref.where('createdDate', '<', date).orderBy('createdDate', 'desc').limit(10)).valueChanges();
    }

    // A little reordering trick to retrieve the immediate previous page
    getPreviousPage = (date: Date) => {
        date = date || new Date();
        return this.afs.collection<IExchangeRate>('exchangeRates',
            ref =>  ref
                    .where('createdDate', '>', date)
                    .orderBy('createdDate', 'asc')
                    .limit(10)).valueChanges();
    }

    // Add new rate doc
    addRate = async (rate: number): Promise<string> => {
        const timestamp = firebase.firestore.FieldValue.serverTimestamp();

        const docData: any = {createdDate: timestamp, rate: rate};
        const doccRef = await this.afs.collection('exchangeRates').add(docData);
        return doccRef.id;
    }
}
