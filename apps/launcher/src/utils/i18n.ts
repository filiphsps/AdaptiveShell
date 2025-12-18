/* eslint-disable import/no-unresolved */

import { getLocales } from 'expo-localization';
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import english from '~/i18n/en.json';

export type i18nKey = keyof typeof english;

const resources = {
    en: { translation: english }
};

const initI18n = async () => {
    // eslint-disable-next-line import/no-named-as-default-member
    i18n.use(initReactI18next).init({
        compatibilityJSON: 'v4',
        resources,
        lng: getLocales()[0]?.languageCode!,
        fallbackLng: 'en',
        supportedLngs: Object.keys(resources),
        cleanCode: true,
        ns: ['translation'],
        defaultNS: 'translation',
        interpolation: {
            escapeValue: false
        }
    });
};

initI18n();

export default i18n;
